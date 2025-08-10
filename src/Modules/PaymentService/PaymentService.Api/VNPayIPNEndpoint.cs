

//using FastEndpoints;
//using Microsoft.Extensions.Logging;
//using Prise;
//using System.Security.Cryptography;
//using System.Text;

//namespace Pay.Core.Endpoints;

//public class VNPayIPNEndpoint : Endpoint<Dictionary<string, string>, object>
//{
//    private readonly IPaymentRepository _paymentRepository;
//    private readonly VNPayConfig _config;
//    private readonly IPluginLoader _paymentPluginLoader;
//    private readonly ILogger<VNPayIPNEndpoint> _logger;

//    public VNPayIPNEndpoint(IPaymentRepository paymentRepository,
//                             VNPayConfig config,
//                             IPluginLoader paymentPluginLoader,
//                             ILogger<VNPayIPNEndpoint> logger)
//    {
//        _paymentRepository = paymentRepository;
//        _config = config;
//        _paymentPluginLoader = paymentPluginLoader;
//        _logger = logger;
//    }

//    public override void Configure()
//    {
//        Post("api/payments/vnpay/ipn");
//        AllowAnonymous();
//        AllowFormData();
//    }

//    public override async Task HandleAsync(Dictionary<string, string> vnpParams, CancellationToken ct)
//    {
//        if (!vnpParams.TryGetValue("vnp_SecureHash", out var secureHash))
//        {
//            await SendAsync("Missing vnp_SecureHash", 400, ct);
//            return;
//        }

//        vnpParams.Remove("vnp_SecureHash");
//        vnpParams.Remove("vnp_SecureHashType");

//        var signData = string.Join("&", vnpParams
//            .OrderBy(kv => kv.Key)
//            .Select(kv => $"{kv.Key}={kv.Value}"));

//        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_config.HashSecret));
//        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
//        var mySecureHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

//        if (!string.Equals(secureHash, mySecureHash, StringComparison.OrdinalIgnoreCase))
//        {
//            await SendAsync("Invalid checksum", 400, ct);
//            return;
//        }

//        if (!vnpParams.TryGetValue("vnp_TxnRef", out var txnRef) ||
//            !Guid.TryParse(txnRef, out var orderId))
//        {
//            await SendAsync("Invalid or missing transaction reference", 400, ct);
//            return;
//        }

//        if (!vnpParams.TryGetValue("vnp_ResponseCode", out var responseCode) ||
//            !vnpParams.TryGetValue("vnp_TransactionNo", out var transactionNo))
//        {
//            await SendAsync("Missing required VNPay parameters", 400, ct);
//            return;
//        }

//        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);
//        if (payment == null)
//        {
//            await SendAsync("The payment not found", 404, ct);
//            return;
//        }

//        var processResult = responseCode == "00"
//            ? payment.ProcessCallback(transactionNo, PaymentResponseCode.Success)
//            : payment.ProcessCallback(transactionNo, PaymentResponseCode.Failed);

//        if (processResult.IsFailed)
//        {
//            await this.ToHttpResultAsync(processResult, ct);
//            return;
//        }

//        await _paymentRepository.SaveChangesAsync(ct);

//        await SendAsync(new { RspCode = "00", Message = "Confirm Success" }, 200, ct);
//    }
//}

