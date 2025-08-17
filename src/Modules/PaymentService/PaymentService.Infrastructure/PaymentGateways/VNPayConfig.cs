namespace PaymentService.Infrastructure.PaymentGateways;

public class VNPayConfig
{
    public string TmnCode { get; set; } // Mã website của merchant trên VNPAY
    public string HashSecret { get; set; } // Secret key để tạo checksum
    public string BasePaymentUrl { get; set; } // URL thanh toán của VNPAY
    public string ReturnUrl { get; set; } // URL nhận kết quả thanh toán
    public string IpnUrl { get; set; } // URL nhận thông báo kết quả thanh toán
}