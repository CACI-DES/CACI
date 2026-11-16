using System;

namespace CACI_Life.Features.Home
{
    public static class TimeHelper
    {
        public static DateTime GetCentralTime()
        {
            var utcNow = DateTime.UtcNow;
            try
            {
                // Windows ID
                var centralZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, centralZone);
            }
            catch (TimeZoneNotFoundException)
            {
                // Linux ID
                var centralZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, centralZone);
            }
        }

        public static DateTime Now => GetCentralTime();
        public static DateTime Today => Now.Date;
    }
}
