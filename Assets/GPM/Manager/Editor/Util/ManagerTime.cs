namespace Gpm.Manager.Util
{
    using System;
    using Constant;

    internal static class ManagerTime
    {
        public static DateTime Now
        {
            get
            {
                return DateTime.UtcNow.AddHours(ManagerInfos.KOREA_STANDARD_TIME);
            }
        }

        public static DateTime ParseTimeInfo(string time)
        {
            return DateTime.ParseExact(time, "yyyy-MM-dd HH:mm", null);
        }

        public static string ToLogString(long tick)
        {
            return ToLogString(new DateTime(tick, DateTimeKind.Utc));
        }

        public static string ToLogString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}