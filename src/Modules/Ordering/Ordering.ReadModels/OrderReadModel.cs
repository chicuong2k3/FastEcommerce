namespace Ordering.ReadModels;

public class OrderReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItemReadModel> Items { get; set; }
    public string Status { get; set; }

    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public decimal Subtotal { get; set; }
    public string PaymentMethod { get; set; }
    public string PhoneNumber { get; set; }
    public string ShippingMethod { get; set; }
    public decimal ShippingCosts { get; set; }
    public string? Street { get; set; }
    public string Ward { get; set; }
    public string District { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }

}
