using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hyip_Payments.Api.Controllers.Report;

[ApiController]
[Route("api/[controller]")]
public class ReportExportController : ControllerBase
{
    public ReportExportController()
    {
        // Set QuestPDF license (Community license is free)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Export report to PDF format
    /// </summary>
    [HttpPost("Export/Pdf")]
    public IActionResult ExportToPdf([FromBody] ReportExportRequest request)
    {
        try
        {
            var pdfBytes = GeneratePdf(request);
            var fileName = $"{SanitizeFileName(request.ReportName)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating PDF", error = ex.Message });
        }
    }

    /// <summary>
    /// Export report to Excel format (CSV-based that Excel can open)
    /// Note: For full XLSX support with formatting, install ClosedXML package
    /// </summary>
    [HttpPost("Export/Excel")]
    public IActionResult ExportToExcel([FromBody] ReportExportRequest request)
    {
        try
        {
            // Generate CSV format that Excel can open directly
            var csvContent = GenerateCsv(request);
            var fileName = $"{SanitizeFileName(request.ReportName)}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating Excel", error = ex.Message });
        }
    }

    /// <summary>
    /// Export report to CSV format
    /// </summary>
    [HttpPost("Export/Csv")]
    public IActionResult ExportToCsv([FromBody] ReportExportRequest request)
    {
        try
        {
            var csvContent = GenerateCsv(request);
            var fileName = $"{SanitizeFileName(request.ReportName)}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating CSV", error = ex.Message });
        }
    }

    #region PDF Generation

    private byte[] GeneratePdf(ReportExportRequest request)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(9));

                // Header
                page.Header().Element(container => ComposeHeader(container, request));

                // Content
                page.Content().Element(container => ComposeContent(container, request));

                // Footer
                page.Footer().Element(container => ComposeFooter(container));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ReportExportRequest request)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(request.ReportName).FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                column.Item().Text($"Report Type: {request.ReportType}").FontSize(10);
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    column.Item().Text($"Period: {request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}").FontSize(9);
                }
            });

            row.ConstantItem(140).Height(50).Placeholder();
        });
    }

    private void ComposeContent(IContainer container, ReportExportRequest request)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Spacing(5);

            // Summary info
            column.Item().Background(Colors.Grey.Lighten3).Padding(5).Text(text =>
            {
                text.Span("Total Records: ").Bold();
                text.Span($"{request.Data.Count}");
                text.Span("    Generated: ").Bold();
                text.Span($"{DateTime.Now:MMM dd, yyyy HH:mm}");
            });

            // Table
            column.Item().Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    foreach (var col in request.Columns)
                    {
                        columns.RelativeColumn();
                    }
                });

                // Header
                table.Header(header =>
                {
                    foreach (var col in request.Columns)
                    {
                        header.Cell().Element(CellStyle).Text(col).Bold();
                    }

                    IContainer CellStyle(IContainer container) =>
                        container.DefaultTextStyle(x => x.FontSize(9))
                                .Background(Colors.Blue.Medium)
                                .Padding(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Medium);
                });

                // Data rows
                foreach (var row in request.Data)
                {
                    foreach (var column in request.Columns)
                    {
                        var value = GetPropertyValue(row, column);
                        table.Cell().Element(CellStyle).Text(value?.ToString() ?? "");
                    }

                    IContainer CellStyle(IContainer container) =>
                        container.BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(5);
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Page ");
            text.CurrentPageNumber();
            text.Span(" of ");
            text.TotalPages();
        });
    }

    #endregion

    #region CSV Generation

    private string GenerateCsv(ReportExportRequest request)
    {
        var sb = new StringBuilder();

        // Add metadata as comments
        sb.AppendLine($"# {request.ReportName}");
        sb.AppendLine($"# Report Type: {request.ReportType}");
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            sb.AppendLine($"# Period: {request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}");
        }
        sb.AppendLine($"# Generated: {DateTime.Now:MMM dd, yyyy HH:mm}");
        sb.AppendLine($"# Total Records: {request.Data.Count}");
        sb.AppendLine();

        // Add header row
        sb.AppendLine(string.Join(",", request.Columns.Select(EscapeCsvValue)));

        // Add data rows
        foreach (var row in request.Data)
        {
            var rowValues = request.Columns.Select(column =>
            {
                var value = GetPropertyValue(row, column);
                return EscapeCsvValue(value?.ToString() ?? "");
            });
            sb.AppendLine(string.Join(",", rowValues));
        }

        return sb.ToString();
    }

    #endregion

    #region Helper Methods

    private object? GetPropertyValue(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out JsonElement propValue))
            {
                return propValue.ValueKind switch
                {
                    JsonValueKind.String => propValue.GetString(),
                    JsonValueKind.Number => propValue.TryGetDecimal(out decimal dec) ? dec : propValue.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => propValue.ToString()
                };
            }
        }
        catch
        {
            // Property not found or error accessing it
        }

        return null;
    }

    private string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        // Escape quotes and wrap in quotes if contains comma, quote, or newline
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Select(c => invalid.Contains(c) ? '_' : c));
    }

    #endregion
}

/// <summary>
/// Request model for report export
/// </summary>
public class ReportExportRequest
{
    public string ReportName { get; set; } = "";
    public string ReportType { get; set; } = "";
    public List<string> Columns { get; set; } = new();
    public List<JsonElement> Data { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? GroupBy { get; set; }
    public string? SortBy { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
