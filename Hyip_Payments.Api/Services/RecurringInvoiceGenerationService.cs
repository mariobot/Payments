using Hyip_Payments.Command.RecurringInvoiceCommand;
using Hyip_Payments.Query.RecurringInvoiceQuery;
using MediatR;

namespace Hyip_Payments.Api.Services;

/// <summary>
/// Background service that automatically generates invoices from recurring invoice templates
/// </summary>
public class RecurringInvoiceGenerationService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RecurringInvoiceGenerationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public RecurringInvoiceGenerationService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RecurringInvoiceGenerationService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Recurring Invoice Generation Service started");

        // Wait 1 minute after startup before first check
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateDueInvoices(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in recurring invoice generation cycle");
            }

            // Wait for next check
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Recurring Invoice Generation Service stopped");
    }

    private async Task GenerateDueInvoices(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        _logger.LogInformation("Checking for recurring invoices due for generation...");

        // Get all recurring invoices that are due
        var dueQuery = new GetRecurringInvoicesDueQuery
        {
            AsOfDate = DateTime.UtcNow.Date
        };

        var dueTemplates = await mediator.Send(dueQuery, cancellationToken);

        if (!dueTemplates.Any())
        {
            _logger.LogInformation("No recurring invoices due for generation");
            return;
        }

        _logger.LogInformation($"Found {dueTemplates.Count} recurring invoice(s) due for generation");

        var successCount = 0;
        var failureCount = 0;

        foreach (var template in dueTemplates)
        {
            try
            {
                _logger.LogInformation($"Generating invoice from template: {template.TemplateName} (ID: {template.Id})");

                var generateCommand = new GenerateInvoiceFromTemplateCommand
                {
                    RecurringInvoiceId = template.Id,
                    InvoiceDate = DateTime.UtcNow,
                    UserId = "SYSTEM" // System-generated
                };

                var result = await mediator.Send(generateCommand, cancellationToken);

                _logger.LogInformation(
                    $"Successfully generated invoice {result.InvoiceNumber} from template {template.TemplateName}. " +
                    $"Amount: ${result.TotalAmount}, Items: {result.Items.Count}");

                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    $"Failed to generate invoice from template: {template.TemplateName} (ID: {template.Id})");
                failureCount++;
            }
        }

        _logger.LogInformation(
            $"Recurring invoice generation cycle complete. " +
            $"Success: {successCount}, Failed: {failureCount}");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recurring Invoice Generation Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
