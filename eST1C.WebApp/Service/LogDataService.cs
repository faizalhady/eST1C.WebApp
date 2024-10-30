using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;

namespace eST1C.WebApp.Service
{
    public class LogDataService
    {
        private readonly LogDbContext _context;

        // Inject the DbContext through the constructor
        public LogDataService(LogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Ensure context is not null
        }

        // DTO to return only the necessary fields (PCName and FrequencyUsed)
        public class LogDataDTO
        {
            public string? PCName { get; set; } // Nullable PCName
            public int FrequencyUsed { get; set; }
            public Dictionary<DateTime, int> DailyLogs { get; set; } = new Dictionary<DateTime, int>();
        }

        public class LastUsedLogDTO
        {
            public string? PCName { get; set; } // Nullable PCName
            public DateTime LastUsed { get; set; }

        }

        public class MonthlyLogDataDTO
        {
            public string? PCName { get; set; } // Nullable PCName
            public Dictionary<string, int> MonthlyCounts { get; set; } = new(); // Initialize dictionary
        }
        public async Task<DateTime?> GetLastUsedDateAsync(string companyName)
        {
            return await _context.ValidLogs
                .Where(p => p.FilePath.Contains(companyName))
                .OrderByDescending(p => p.Timestamp)
                .Select(p => p.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<DateTime?> GetMostRecentLastUsedAsync(string workcell)
        {
            return await _context.LastUsed
                .Where(log => log.Workcell.Contains(workcell))
                .MaxAsync(log => (DateTime?)log.LastUsed);
        }


        // Method to fetch and return data from the database for the entire log history
        public async Task<List<LogDataDTO>> GetLogDataAsync()
        {
            return await _context.ValidLogs
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    FrequencyUsed = group.Count()
                })
                .ToListAsync();
        }

        // Method to fetch logs filtered by a specific week (YYYY-WW format)
        public async Task<List<LogDataDTO>> GetLogDataByWeekAsync(string week)
        {
            if (string.IsNullOrEmpty(week)) return new List<LogDataDTO>();

            // Extract year and week from the input string
            var parts = week.Split('-');
            if (parts.Length != 2) return new List<LogDataDTO>();

            if (!int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int weekNumber)) return new List<LogDataDTO>();

            // Calculate start and end date for the given week number
            var firstDayOfYear = new DateTime(year, 1, 1);
            var startOfWeek = firstDayOfYear.AddDays((weekNumber - 1) * 7);
            var endOfWeek = startOfWeek.AddDays(7);

            return await _context.ValidLogs
                .Where(log => log.Timestamp >= startOfWeek && log.Timestamp < endOfWeek)
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    FrequencyUsed = group.Count()
                })
                .ToListAsync();
        }

        // Method to fetch logs filtered by a specific month (YYYY-MM format)
        public async Task<List<LogDataDTO>> GetLogDataByMonthAsync(string month)
        {
            if (string.IsNullOrEmpty(month)) return new List<LogDataDTO>();

            var parts = month.Split('-');
            if (parts.Length != 2) return new List<LogDataDTO>();

            if (!int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int monthNumber)) return new List<LogDataDTO>();

            // Calculate the start and end date for the month
            var startOfMonth = new DateTime(year, monthNumber, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            return await _context.ValidLogs
                .Where(log => log.Timestamp >= startOfMonth && log.Timestamp < endOfMonth)
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    FrequencyUsed = group.Count()
                })
                .ToListAsync();
        }

        // Method to fetch the most recent log for each PC
        public async Task<List<LastUsedLogDTO>> GetLastUsedLogsAsync(string workcell)
        {
            return await _context.LastUsed
                .Where(log => log.Workcell.Contains(workcell)) // Filter by Workcell
                .GroupBy(log => log.PCName)
                .Select(group => new LastUsedLogDTO
                {
                    PCName = group.Key, // 'group.Key' is the PCName
                    LastUsed = group.Max(log => log.LastUsed) // Get the max LastUse for each PCName
                })
                .ToListAsync();
        }


        // Method to fetch logs grouped by month
        public async Task<List<MonthlyLogDataDTO>> GetMonthlyLogDataAsync()
        {
            var result = await _context.ValidLogs
                .GroupBy(log => new { log.PCName, log.Timestamp.Year, log.Timestamp.Month })
                .Select(group => new
                {
                    group.Key.PCName,
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    Count = group.Count()
                })
                .ToListAsync();

            return result
                .GroupBy(data => data.PCName)
                .Select(group => new MonthlyLogDataDTO
                {
                    PCName = group.Key,
                    MonthlyCounts = group.ToDictionary(
                        item => new DateTime(item.Year, item.Month, 1).ToString("MMM yyyy"),
                        item => item.Count
                    )
                })
                .ToList();
        }

        public async Task<List<LogDataDTO>> GetLogDataByDayAsync(DateTime day)
        {
            var startOfDay = day.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.ValidLogs
                .Where(log => log.Timestamp >= startOfDay && log.Timestamp < endOfDay)
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    FrequencyUsed = group.Count()
                })
                .ToListAsync();
        }
    }
}
