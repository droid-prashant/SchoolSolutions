using Application.Notifications.Dtos;
using Application.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Services
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly NotificationConnectionTracker _connectionTracker;

        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext,
            NotificationConnectionTracker connectionTracker)
        {
            _hubContext = hubContext;
            _connectionTracker = connectionTracker;
        }

        public async Task<bool> SendToUserAsync(NotificationDispatchDto notification, CancellationToken cancellationToken)
        {
            if (!_connectionTracker.IsOnline(notification.UserId.ToString()))
            {
                return false;
            }

            await _hubContext.Clients
                .Group($"user-{notification.UserId}")
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            return true;
        }
    }
}
