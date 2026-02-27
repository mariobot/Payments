using Hyip_Payments.Services;
using MediatR;

namespace Hyip_Payments.Command.InvoiceCommand;

/// <summary>
/// Domain event published when an invoice is created
/// </summary>
public class InvoiceCreatedEvent : INotification
{
    public int InvoiceId { get; set; }
    public int? CustomerId { get; set; }

    public InvoiceCreatedEvent(int invoiceId, int? customerId)
    {
        InvoiceId = invoiceId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Domain event published when an invoice is updated
/// </summary>
public class InvoiceUpdatedEvent : INotification
{
    public int InvoiceId { get; set; }
    public int? OldCustomerId { get; set; }
    public int? NewCustomerId { get; set; }

    public InvoiceUpdatedEvent(int invoiceId, int? oldCustomerId, int? newCustomerId)
    {
        InvoiceId = invoiceId;
        OldCustomerId = oldCustomerId;
        NewCustomerId = newCustomerId;
    }
}

/// <summary>
/// Domain event published when an invoice is deleted
/// </summary>
public class InvoiceDeletedEvent : INotification
{
    public int InvoiceId { get; set; }
    public int? CustomerId { get; set; }

    public InvoiceDeletedEvent(int invoiceId, int? customerId)
    {
        InvoiceId = invoiceId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Handler for InvoiceCreatedEvent - Updates customer balance
/// </summary>
public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public InvoiceCreatedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(InvoiceCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CustomerId.HasValue)
        {
            await _balanceService.UpdateCustomerBalanceAsync(notification.CustomerId.Value, cancellationToken);
        }
    }
}

/// <summary>
/// Handler for InvoiceUpdatedEvent - Updates both old and new customer balances
/// </summary>
public class InvoiceUpdatedEventHandler : INotificationHandler<InvoiceUpdatedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public InvoiceUpdatedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(InvoiceUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var customerIdsToUpdate = new List<int>();

        // Update old customer if changed
        if (notification.OldCustomerId.HasValue && notification.OldCustomerId != notification.NewCustomerId)
        {
            customerIdsToUpdate.Add(notification.OldCustomerId.Value);
        }

        // Update new customer
        if (notification.NewCustomerId.HasValue)
        {
            customerIdsToUpdate.Add(notification.NewCustomerId.Value);
        }

        await _balanceService.UpdateMultipleCustomerBalancesAsync(customerIdsToUpdate, cancellationToken);
    }
}

/// <summary>
/// Handler for InvoiceDeletedEvent - Updates customer balance
/// </summary>
public class InvoiceDeletedEventHandler : INotificationHandler<InvoiceDeletedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public InvoiceDeletedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(InvoiceDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CustomerId.HasValue)
        {
            await _balanceService.UpdateCustomerBalanceAsync(notification.CustomerId.Value, cancellationToken);
        }
    }
}
