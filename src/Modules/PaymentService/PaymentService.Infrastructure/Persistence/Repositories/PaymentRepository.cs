using Microsoft.EntityFrameworkCore;
using PaymentService.Core.Entities;
using PaymentService.Core.Repositories;
using Shared.Infrastructure.Repositories;

namespace PaymentService.Infrastructure.Persistence.Repositories;

internal class PaymentRepository : RepositoryBase<Payment, Guid>, IPaymentRepository
{
    private readonly PaymentDbContext _dbContext;

    public PaymentRepository(PaymentDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }
}
