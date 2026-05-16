using Application.Notifications.Dtos;
using Application.Notifications.ViewModels;

namespace Application.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task RegisterTokenAsync(RegisterNotificationTokenDto request, CancellationToken cancellationToken);
        Task DeactivateTokenAsync(DeactivateNotificationTokenDto request, CancellationToken cancellationToken);
        Task<List<NotificationViewModel>> GetMyNotificationsAsync(CancellationToken cancellationToken);
        Task<NoticeLetterViewModel> GetMyNoticeLetterAsync(string noticeId, CancellationToken cancellationToken);
        Task<int> GetUnreadCountAsync(CancellationToken cancellationToken);
        Task MarkAsReadAsync(string userNotificationId, CancellationToken cancellationToken);
        Task MarkAllAsReadAsync(CancellationToken cancellationToken);
        Task CreateAttendanceNotificationsAsync(IReadOnlyCollection<Guid> attendanceIds, CancellationToken cancellationToken);
        Task<List<NotificationDispatchDto>> CreateNoticeNotificationAsync(Guid noticeId, CancellationToken cancellationToken);
        Task DispatchAsync(IReadOnlyCollection<NotificationDispatchDto> notifications, CancellationToken cancellationToken);
    }
}
