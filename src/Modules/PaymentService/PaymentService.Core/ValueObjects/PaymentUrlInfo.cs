namespace PaymentService.Core.ValueObjects;

public record PaymentUrlInfo(
    string PaymentUrl,
    string PaymentToken,
    string? QrCode
);
