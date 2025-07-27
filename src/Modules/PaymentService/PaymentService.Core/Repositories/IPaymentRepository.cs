

namespace PaymentService.Core.Repositories;

public interface IPaymentRepository : IRepositoryBase<Payment, Guid>
{
    Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
