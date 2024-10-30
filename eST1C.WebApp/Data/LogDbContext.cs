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
        public DbSet<LastUsedLog> LastUsed { get; set; }
        
    }

    public class ValidLogs
    {
        public int Id { get; set; } 
        public string? PCName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? FilePath { get; set; }
    }
    public class PCNameCount
    {
        public int Id { get; set; } 
        public string? CompanyName { get; set; }
        public int UniquePCCount { get; set; }
    }
    public class LastUsedLog
    {
        public int Id { get; set; } 
        public string? Workcell { get; set; }
        public string? PCName { get; set; }
        
        public DateTime LastUsed { get; set; }
    }
}
