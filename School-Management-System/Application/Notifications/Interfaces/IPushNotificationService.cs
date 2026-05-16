using Application.Notifications.Dtos;

namespace Application.Notifications.Interfaces
{
    public interface IPushNotificationService
    {
        Task<PushNotificationResultDto> SendToUserAsync(NotificationDispatchDto notification, CancellationToken cancellationToken);
    }
}
