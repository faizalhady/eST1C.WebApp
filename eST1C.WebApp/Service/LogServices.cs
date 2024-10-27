using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;
using System.Globalization;


namespace eST1C.WebApp.Service
{
    public class LogServices
    {
        private readonly LogDbContext _context;

        public LogServices(LogDbContext context)
        {
            _context = context;
        }

        public class LogDataDTO
        {
            public string? PCName { get; set; }
            public int FrequencyUsed { get; set; }
            public Dictionary<DateTime, int> DailyLogs { get; set; } = new Dictionary<DateTime, int>();
        }

        public async Task<List<LogDataDTO>> GetLogDataByDateRangeAsync(DateTime from, DateTime to, string companyName)
        {
            // Fetch logs within the specified date range and filter by company name
            var logs = await _context.ValidLogs
                .Where(log => log.Timestamp >= from && log.Timestamp <= to &&
                              log.FilePath.Contains(companyName))
                .GroupBy(log => new { log.PCName, Date = log.Timestamp.Date })
                .Select(group => new
                {
                    PCName = group.Key.PCName,
                    Date = group.Key.Date,
                    Frequency = group.Count()
                })
                .ToListAsync();

            // If no logs found, log for debugging
            if (!logs.Any())
            {
                Console.WriteLine("No logs found for the given date range and company.");
            }

            // Transform the result into a list of LogDataDTO
            var result = logs
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    DailyLogs = group.ToDictionary(log => log.Date, log => log.Frequency)
                })
                .ToList();

            // Debug output
            foreach (var logData in result)
            {
                Console.WriteLine($"PC Name: {logData.PCName}");
                foreach (var dailyLog in logData.DailyLogs)
                {
                    Console.WriteLine($"Date: {dailyLog.Key}, Count: {dailyLog.Value}");
                }
            }

            return result;
        }

        public async Task<List<LogDataDTO>> GetLogDataByMonthRangeAsync(DateTime fromMonth, DateTime toMonth, string companyName)
        {
            // Calculate the start and end dates for the month range
            var startOfMonth = new DateTime(fromMonth.Year, fromMonth.Month, 1);
            var endOfMonth = new DateTime(toMonth.Year, toMonth.Month, 1).AddMonths(1).AddDays(-1);

            // Fetch logs grouped by PCName and month within the specified range
            var logs = await _context.ValidLogs
                .Where(log => log.Timestamp >= startOfMonth && log.Timestamp <= endOfMonth &&
                              log.FilePath.Contains(companyName))
                .GroupBy(log => new { log.PCName, Year = log.Timestamp.Year, Month = log.Timestamp.Month })
                .Select(group => new
                {
                    PCName = group.Key.PCName,
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    Frequency = group.Count()
                })
                .ToListAsync();

            // If no logs found, log for debugging
            if (!logs.Any())
            {
                Console.WriteLine("No logs found for the given month range and company.");
            }

            // Transform the result into a list of LogDataDTO
            var result = logs
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    DailyLogs = group.ToDictionary(
                        log => new DateTime(log.Year, log.Month, 1), // Representing the start of each month
                        log => log.Frequency)
                })
                .ToList();

            // Debug output
            foreach (var logData in result)
            {
                Console.WriteLine($"PC Name: {logData.PCName}");
                foreach (var monthlyLog in logData.DailyLogs)
                {
                    Console.WriteLine($"Month: {monthlyLog.Key:MM/yyyy}, Count: {monthlyLog.Value}");
                }
            }

            return result;
        }

        public async Task<List<LogDataDTO>> GetLogDataByWeekRangeAsync(DateTime fromWeek, DateTime toWeek, string companyName)
        {
            // Calculate the start and end dates for the week range
            var startOfWeek = GetFirstDateOfWeekISO8601(fromWeek);
            var endOfWeek = GetFirstDateOfWeekISO8601(toWeek).AddDays(6);

            // Fetch logs within the specified range first
            var logs = await _context.ValidLogs
                .Where(log => log.Timestamp >= startOfWeek && log.Timestamp <= endOfWeek &&
                              log.FilePath.Contains(companyName))
                .ToListAsync();  // Fetch logs to memory

            // Perform grouping and transformation in memory
            var groupedLogs = logs
                .GroupBy(log => new
                {
                    log.PCName,
                    Year = log.Timestamp.Year,
                    Week = GetIso8601WeekOfYear(log.Timestamp)
                })
                .Select(group => new
                {
                    PCName = group.Key.PCName,
                    Year = group.Key.Year,
                    Week = group.Key.Week,
                    Frequency = group.Count()
                })
                .ToList();

            // Transform the result into a list of LogDataDTO
            var result = groupedLogs
                .GroupBy(log => log.PCName)
                .Select(group => new LogDataDTO
                {
                    PCName = group.Key,
                    DailyLogs = group.ToDictionary(
                        log => GetFirstDateOfWeekISO8601(new DateTime(log.Year, 1, 1)).AddDays((log.Week - 1) * 7),
                        log => log.Frequency)
                })
                .ToList();

            return result;
        }


        // Helper method to get the first day of the week (ISO 8601 standard)
        private DateTime GetFirstDateOfWeekISO8601(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var difference = dayOfWeek == 0 ? -6 : 1 - dayOfWeek; // Adjust if Sunday (0) or other days
            return date.AddDays(difference);
        }

        // Helper method to get the ISO 8601 week number of a date
        private int GetIso8601WeekOfYear(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            if (day == 0) day = 7; // Adjust if it's Sunday

            // Get the week number
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
