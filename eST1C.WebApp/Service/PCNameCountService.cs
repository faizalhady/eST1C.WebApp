using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;
using eST1C.WebApp.DTO;

namespace eST1C.WebApp.Service 
{
    public class PCNameCountService
    {
        private readonly LogDbContext _context;

        public PCNameCountService(LogDbContext context)
        {
            _context = context;
        }

        public async Task<List<PCNameCountDTO>> GetPCNameCountsAsync()
        {
            var pcNameCounts = await _context.PCNameCount
                .Select(p => new PCNameCountDTO
                {
                    CompanyName = p.CompanyName,
                    UniquePCCount = p.UniquePCCount
                })
                .ToListAsync();

            return pcNameCounts;
        }
    }
}
