using Hyip_Payments.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace Hyip_Payments.Web.Client.Services;

/// <summary>
/// Generic service for exporting reports to various formats (PDF, Excel, CSV)
/// </summary>
public class ReportExportService
{
    private readonly HttpClient _httpClient;

    public ReportExportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Export report data to PDF format
    /// </summary>
    public async Task<byte[]?> ExportToPdfAsync(ReportExportRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Report/Export/Pdf", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to PDF: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Export report data to Excel format
    /// </summary>
    public async Task<byte[]?> ExportToExcelAsync(ReportExportRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Report/Export/Excel", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to Excel: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Export report data to CSV format
    /// </summary>
    public string ExportToCsv(ReportExportRequest request)
    {
        var sb = new StringBuilder();

        // Add header row
        sb.AppendLine(string.Join(",", request.Columns.Select(c => EscapeCsvValue(c))));

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

    /// <summary>
    /// Download file to browser
    /// </summary>
    public async Task DownloadFileAsync(byte[] fileBytes, string fileName, string mimeType)
    {
        // This will be implemented with JSInterop in the component
        await Task.CompletedTask;
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

    private object? GetPropertyValue(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out JsonElement propValue))
            {
                return propValue.ValueKind switch
                {
                    JsonValueKind.String => propValue.GetString(),
                    JsonValueKind.Number => propValue.GetDecimal(),
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
