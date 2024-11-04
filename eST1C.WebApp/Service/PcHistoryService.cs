using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;  // Using the existing models in LogDbContext

public class PcHistoryService
{
    private readonly LogDbContext _context;

    public PcHistoryService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<List<PCMigrationSummary>> GetPCMigrationSummariesAsync()
{
    return await _context.PCMigrationHistory
        .GroupBy(m => m.PCName)
        .Select(g => new PCMigrationSummary
        {
            PCName = g.Key,
            MaxMigrationStep = g.Max(m => m.MigrationStep)
        })
        .OrderByDescending(m => m.MaxMigrationStep)  // Order by MaxMigrationStep in descending order
        .ToListAsync();
}


}

public class PCMigrationSummary
{
    public string PCName { get; set; }
    public long MaxMigrationStep { get; set; }  // Change this to long as well
}

