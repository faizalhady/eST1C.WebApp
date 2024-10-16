using Microsoft.EntityFrameworkCore;

namespace eST1C.WebApp.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        // This assumes your table in the database is named 'LogData'
        // and you don't need to define a model class because it's already there.
        public DbSet<LogData> LogData { get; set; }
    }

    // The 'LogData' class may not be necessary if you're scaffolding models from the database.
    // However, if you need to map specific fields in the table, it can look like this:
    public class LogData
    {
        public int Id { get; set; } // Primary Key, assuming there is one
        public string PCName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
