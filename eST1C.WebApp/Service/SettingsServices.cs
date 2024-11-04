using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eST1C.WebApp.Data;

namespace eST1C.WebApp.Service
{
    public class SettingsServices
    {
        private readonly LogDbContext _context;

        public SettingsServices(LogDbContext context)
        {
            _context = context;
        }

        public async Task UpdateInactiveThresholdAsync(int months)
        {
            // Find existing setting, or create a new one if it doesn't exist
            var setting = _context.Settings.FirstOrDefault(s => s.SettingName == "InactiveThreshold");

            if (setting == null)
            {
                // Create new setting if it doesn't exist
                setting = new Setting
                {
                    SettingName = "InactiveThreshold",
                    SettingValue = months.ToString()
                };
                _context.Settings.Add(setting);
            }
            else
            {
                // Update the SettingValue if it already exists
                setting.SettingValue = months.ToString();
                _context.Settings.Update(setting);
            }

            // Save changes to the database
            Console.WriteLine("stting saved");
            await _context.SaveChangesAsync();
        }

         public async Task<int> GetInactiveThresholdAsync()
        {
            var setting = await _context.Settings
                .Where(s => s.SettingName == "InactiveThreshold")
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync();

            return int.TryParse(setting, out var months) ? months : 6; // Default to 6 if not found or invalid
        }
    }
}
