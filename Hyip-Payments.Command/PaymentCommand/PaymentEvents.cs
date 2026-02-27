using Hyip_Payments.Services;
using MediatR;

namespace Hyip_Payments.Command.PaymentCommand;

/// <summary>
/// Domain event published when a payment is made
/// </summary>
public class PaymentCreatedEvent : INotification
{
    public int PaymentId { get; set; }
    public int? InvoiceId { get; set; }
    public int? CustomerId { get; set; }

    public PaymentCreatedEvent(int paymentId, int? invoiceId, int? customerId)
    {
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Domain event published when a payment is updated
/// </summary>
public class PaymentUpdatedEvent : INotification
{
    public int PaymentId { get; set; }
    public int? CustomerId { get; set; }

    public PaymentUpdatedEvent(int paymentId, int? customerId)
    {
        PaymentId = paymentId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Domain event published when a payment is deleted
/// </summary>
public class PaymentDeletedEvent : INotification
{
    public int PaymentId { get; set; }
    public int? CustomerId { get; set; }

    public PaymentDeletedEvent(int paymentId, int? customerId)
    {
        PaymentId = paymentId;
        CustomerId = customerId;
    }
}

/// <summary>
/// Handler for PaymentCreatedEvent - Updates customer balance and invoice status
/// </summary>
public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public PaymentCreatedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CustomerId.HasValue)
        {
            await _balanceService.UpdateCustomerBalanceAsync(notification.CustomerId.Value, cancellationToken);
        }
    }
}

/// <summary>
/// Handler for PaymentUpdatedEvent - Updates customer balance
/// </summary>
public class PaymentUpdatedEventHandler : INotificationHandler<PaymentUpdatedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public PaymentUpdatedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(PaymentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CustomerId.HasValue)
        {
            await _balanceService.UpdateCustomerBalanceAsync(notification.CustomerId.Value, cancellationToken);
        }
    }
}

/// <summary>
/// Handler for PaymentDeletedEvent - Updates customer balance
/// </summary>
public class PaymentDeletedEventHandler : INotificationHandler<PaymentDeletedEvent>
{
    private readonly ICustomerBalanceService _balanceService;

    public PaymentDeletedEventHandler(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    public async Task Handle(PaymentDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CustomerId.HasValue)
        {
            await _balanceService.UpdateCustomerBalanceAsync(notification.CustomerId.Value, cancellationToken);
        }
    }
}
