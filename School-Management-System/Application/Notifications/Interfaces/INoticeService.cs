using Application.Notifications.Dtos;
using Application.Notifications.ViewModels;

namespace Application.Notifications.Interfaces
{
    public interface INoticeService
    {
        Task<NoticeViewModel> CreateAsync(CreateNoticeDto request, CancellationToken cancellationToken);
        Task<NoticeViewModel> UpdateAsync(string noticeId, UpdateNoticeDto request, CancellationToken cancellationToken);
        Task PublishAsync(string noticeId, CancellationToken cancellationToken);
        Task<List<NoticeViewModel>> GetAllAsync(CancellationToken cancellationToken);
        Task<NoticeViewModel> GetByIdAsync(string noticeId, CancellationToken cancellationToken);
    }
}
