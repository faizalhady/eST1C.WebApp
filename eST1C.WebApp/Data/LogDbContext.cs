using Microsoft.EntityFrameworkCore;

namespace eST1C.WebApp.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        public DbSet<ValidLogs> ValidLogs { get; set; }
        public DbSet<PCNameCount> PCNameCount { get; set; }
    }

    public class ValidLogs
    {
        public int Id { get; set; } 
        public string? PCName { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class PCNameCount
    {
        public int Id { get; set; } 
        public string? CompanyName { get; set; }
        public int UniquePCCount { get; set; }
    }
}
