namespace PaymentService.Core.Events;

public record PaymentCanceled(Guid OrderId) : DomainEvent;
