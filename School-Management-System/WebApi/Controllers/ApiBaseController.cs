using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        private ISender _mediator = null;
        protected ISender Mediator => _mediator??=HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
