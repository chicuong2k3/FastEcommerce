namespace PaymentService.Core.Events;

public record PaymentSucceeded(Guid OrderId) : DomainEvent;