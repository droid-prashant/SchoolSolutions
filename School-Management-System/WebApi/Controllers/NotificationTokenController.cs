using Application.Notifications.Dtos;
using Application.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationTokenController : ApiBaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationTokenController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("register-token")]
        public async Task RegisterToken([FromBody] RegisterNotificationTokenDto request, CancellationToken cancellationToken)
        {
            await _notificationService.RegisterTokenAsync(request, cancellationToken);
        }

        [HttpPost("deactivate-token")]
        public async Task DeactivateToken([FromBody] DeactivateNotificationTokenDto request, CancellationToken cancellationToken)
        {
            await _notificationService.DeactivateTokenAsync(request, cancellationToken);
        }
    }
}
