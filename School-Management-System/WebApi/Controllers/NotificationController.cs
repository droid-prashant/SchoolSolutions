using Application.Notifications.Interfaces;
using Application.Notifications.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ApiBaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my")]
        public async Task<List<NotificationViewModel>> GetMyNotifications(CancellationToken cancellationToken)
        {
            return await _notificationService.GetMyNotificationsAsync(cancellationToken);
        }

        [HttpGet("notices/{noticeId}/letter")]
        public async Task<NoticeLetterViewModel> GetMyNoticeLetter(string noticeId, CancellationToken cancellationToken)
        {
            return await _notificationService.GetMyNoticeLetterAsync(noticeId, cancellationToken);
        }

        [HttpGet("unread-count")]
        public async Task<int> GetUnreadCount(CancellationToken cancellationToken)
        {
            return await _notificationService.GetUnreadCountAsync(cancellationToken);
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task MarkAsRead(string id, CancellationToken cancellationToken)
        {
            await _notificationService.MarkAsReadAsync(id, cancellationToken);
        }

        [HttpPost("mark-all-as-read")]
        public async Task MarkAllAsRead(CancellationToken cancellationToken)
        {
            await _notificationService.MarkAllAsReadAsync(cancellationToken);
        }
    }
}
