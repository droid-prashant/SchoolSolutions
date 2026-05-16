using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApi.Services;

namespace WebApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly NotificationConnectionTracker _connectionTracker;

        public NotificationHub(NotificationConnectionTracker connectionTracker)
        {
            _connectionTracker = connectionTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Context.User?.FindFirst("userId")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                _connectionTracker.Add(userId, Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Context.User?.FindFirst("userId")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                _connectionTracker.Remove(userId, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
