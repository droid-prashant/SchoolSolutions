using Application.Master.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Master.Interface
{
    public interface IMasterDataService
    {
        Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken);
    }
}
