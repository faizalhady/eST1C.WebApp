using System.ComponentModel.DataAnnotations;
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
        public DbSet<WorkcellPcCount> WorkcellPcCount { get; set; }
        public DbSet<LastUsedLog> LastUsed { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<PCMigrationHistory> PCMigrationHistory { get; set; }
        
    }

    public class ValidLogs
    {
        public int Id { get; set; } 
        public string? PCName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? FilePath { get; set; }
    }
    public class WorkcellPcCount
    {
        public int Id { get; set; } 
        public string? Workcell { get; set; }
        public int PcCount { get; set; }
    }
    public class LastUsedLog
    {
        public int Id { get; set; } 
        public string? Workcell { get; set; }
        public string? PCName { get; set; }
        
        public DateTime LastUsed { get; set; }
    }

    public class Setting
    {
        [Key]
        public string SettingName { get; set; }
        public string? SettingValue { get; set; }
    }

    public class PCMigrationHistory
{
    [Key]
    public int ID { get; set; }  // Primary Key

    [Required]
    [StringLength(100)]
    public string PCName { get; set; }

    [Required]
    [StringLength(100)]
    public string Workcell { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    public long MigrationStep { get; set; }  // Change this from int to long

    [StringLength(100)]
    public string PreviousWorkcell { get; set; }  // Nullable if the first entry
}

}
