using NotificationService.Core;
using Shared.Infrastructure.Repositories;

namespace NotificationService.Infrastructure;

internal class NotificationRepository : RepositoryBase<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext dbContext) : base(dbContext)
    {
    }
}
