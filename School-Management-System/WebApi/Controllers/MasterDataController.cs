using Application.Master.Interface;
using Application.Master.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ApiBaseController
    {
        private readonly IMasterDataService _masterDataService;
        public MasterDataController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }
        [HttpGet]
        [Route("GetAllProvince")]
        public async Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken)
        {
            var result = await _masterDataService.GetAllProvince(cancellationToken);
            return result;
        }
    }
}
