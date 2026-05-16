using Application.Notifications.Dtos;

namespace Application.Notifications.Interfaces
{
    public interface ISignalRNotificationService
    {
        Task<bool> SendToUserAsync(NotificationDispatchDto notification, CancellationToken cancellationToken);
    }
}
