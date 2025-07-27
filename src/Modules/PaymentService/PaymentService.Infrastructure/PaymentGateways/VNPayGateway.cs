using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Core.Entities;
using PaymentService.Core.PaymentGateways;
using PaymentService.Core.ValueObjects;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PaymentService.Infrastructure.PaymentGateways;

internal class VNPayGateway : IPaymentGateway
{
    private readonly VNPayConfig config;
    private readonly ILogger<VNPayGateway> logger;

    public VNPayGateway(IOptions<VNPayConfig> options, ILogger<VNPayGateway> logger)
    {
        config = options.Value;
        this.logger = logger;
    }

    public string Reference => "VNPAY_GATEWAY";

    public Result<PaymentUrlInfo> CreatePaymentUrl(Payment payment)
    {
        try
        {
            var vnpParams = new Dictionary<string, string>
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = config.TmnCode,
                ["vnp_Amount"] = (payment.TotalAmount.Amount * 100).ToString(),
                ["vnp_CreateDate"] = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = "::1",
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = $"Thanh toan don hang: {payment.OrderId}",
                ["vnp_OrderType"] = "other",
                ["vnp_ReturnUrl"] = config.ReturnUrl,
                ["vnp_TxnRef"] = payment.OrderId.ToString()
            };

            var signData = string.Join("&", vnpParams
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(config.HashSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
            var secureHash = BitConverter.ToString(hash).Replace("-", "").ToLower();

            vnpParams.Add("vnp_SecureHash", secureHash);

            var queryString = string.Join("&", vnpParams
                .Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));
            var paymentUrl = $"{config.BasePaymentUrl}?{queryString}";

            return Result.Ok(new PaymentUrlInfo(
                paymentUrl,
                secureHash,
                null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating VNPAY payment URL");
            return Result.Fail("Failed to create payment URL");
        }
    }

    public Task<Result> RefundAsync(Payment payment, decimal refundAmount, string refundReason, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
