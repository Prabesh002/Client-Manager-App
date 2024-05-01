using System;
namespace Client_Manager_App
{
    public static class DateTimeExtensions
    {
        public static DateTime ToNepalTime(this DateTime dateTimeUtc)
        {
            var nepalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, nepalTimeZone);
        }
    }
}
