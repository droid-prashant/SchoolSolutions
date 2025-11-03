using Application.Common.Interfaces;
using Application.Master.Interface;
using Application.Master.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.MasterData
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IApplicationDbContext _dbContext;
        public MasterDataService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ProvinceViewModel>> GetAllProvince(CancellationToken cancellationToken)
        {
            var result = await _dbContext.Provinces.Select(x => new ProvinceViewModel
            {
                Id = x.Id,
                Name = x.ProvinceName,
                Districts = x.Districts.Select(y => new DistrictViewModel
                {
                    Id = y.Id,
                    Name = y.DistrictName,
                    Municipalities = y.Municipalities.Select(m => new MunicipalityViewModel
                    {
                        Id = m.Id,
                        Name = m.MunicipalityName
                    }).ToList()
                }).ToList()
            }).ToListAsync(cancellationToken);

            return result;
        }
    }
}
