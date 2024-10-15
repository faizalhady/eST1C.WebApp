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
            _context = context;
        }

        // DTO to return only the necessary fields (PCName and FrequencyUsed)
        public class LogDataDTO
        {
            public string PCName { get; set; }
            public int FrequencyUsed { get; set; }
        }

        public class LastUsedLogDTO
        {
            public string PCName { get; set; }
            public DateTime LastUsed { get; set; }
        }

        public class MonthlyLogDataDTO
        {
            public string PCName { get; set; }
            public Dictionary<string, int> MonthlyCounts { get; set; }  // A dictionary to hold month-year as key and count as value
        }

        // Method to fetch and return data from the database for the entire log history
        public async Task<List<LogDataDTO>> GetLogDataAsync()
        {
            return await _context.LogData
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    FrequencyUsed = group.Count() // Count how many times this PCName appears
                })
                .ToListAsync();
        }

        // Method to fetch logs filtered by a specific week (YYYY-WW format)
        public async Task<List<LogDataDTO>> GetLogDataByWeekAsync(string week)
        {
            // Extract year and week from the input string
            var parts = week.Split('-');
            int year = int.Parse(parts[0]);
            int weekNumber = int.Parse(parts[1]);

            // Calculate start and end date for the given week number
            var firstDayOfYear = new DateTime(year, 1, 1);
            var startOfWeek = firstDayOfYear.AddDays((weekNumber - 1) * 7);
            var endOfWeek = startOfWeek.AddDays(7);

            // Fetch logs where the timestamp falls within the given week range
            return await _context.LogData
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
            // Extract year and month from the input string
            var parts = month.Split('-');
            int year = int.Parse(parts[0]);
            int monthNumber = int.Parse(parts[1]);

            // Calculate the start and end date for the month
            var startOfMonth = new DateTime(year, monthNumber, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            // Fetch logs where the timestamp falls within the given month range
            return await _context.LogData
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
        public async Task<List<LastUsedLogDTO>> GetLastUsedLogsAsync()
        {
            return await _context.LogData
                .GroupBy(log => log.PCName)
                .Select(group => new LastUsedLogDTO
                {
                    PCName = group.Key,
                    LastUsed = group.Max(log => log.Timestamp) // Get the most recent timestamp
                })
                .ToListAsync();
        }

        // Method to fetch logs grouped by month
        public async Task<List<MonthlyLogDataDTO>> GetMonthlyLogDataAsync()
        {
            return await _context.LogData
                .GroupBy(log => new { log.PCName, log.Timestamp.Year, log.Timestamp.Month })
                .Select(group => new
                {
                    group.Key.PCName,
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    Count = group.Count()
                })
                .ToListAsync()
                .ContinueWith(task => task.Result
                    .GroupBy(data => data.PCName)
                    .Select(group => new MonthlyLogDataDTO
                    {
                        PCName = group.Key,
                        MonthlyCounts = group.ToDictionary(
                            item => new DateTime(item.Year, item.Month, 1).ToString("MMM yyyy"),
                            item => item.Count
                        )
                    })
                    .ToList()
                );
        }
    }
}
