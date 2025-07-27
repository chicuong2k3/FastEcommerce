namespace PaymentService.Core.Events;

public record PaymentRefunded(Guid PaymentId, decimal Amount, string Reason) : DomainEvent;
