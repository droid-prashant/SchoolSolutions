using Application.Notifications.Dtos;
using Application.Notifications.Interfaces;
using Application.Notifications.ViewModels;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [HasPermission(PermissionNames.NoticeManage)]
    [Route("api/notices")]
    public class NoticeController : ApiBaseController
    {
        private readonly INoticeService _noticeService;

        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        [HttpPost]
        public async Task<NoticeViewModel> Create([FromBody] CreateNoticeDto request, CancellationToken cancellationToken)
        {
            return await _noticeService.CreateAsync(request, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<NoticeViewModel> Update(string id, [FromBody] UpdateNoticeDto request, CancellationToken cancellationToken)
        {
            return await _noticeService.UpdateAsync(id, request, cancellationToken);
        }

        [HttpPost("{id}/publish")]
        public async Task Publish(string id, CancellationToken cancellationToken)
        {
            await _noticeService.PublishAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<List<NoticeViewModel>> GetAll(CancellationToken cancellationToken)
        {
            return await _noticeService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<NoticeViewModel> GetById(string id, CancellationToken cancellationToken)
        {
            return await _noticeService.GetByIdAsync(id, cancellationToken);
        }
    }
}
