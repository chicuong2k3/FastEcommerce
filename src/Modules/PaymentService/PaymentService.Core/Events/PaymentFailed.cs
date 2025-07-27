namespace PaymentService.Core.Events;

public record PaymentFailed(Guid OrderId) : DomainEvent;