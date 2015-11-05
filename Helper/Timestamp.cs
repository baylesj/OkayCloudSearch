using System;
using System.Globalization;

namespace OkayCloudSearch.Helper
{
    public static class Timestamp
    {
        public static int CurrentTimestamp()
        {
            return DateToTimestamp(DateTime.Now);
        }

        public static int DateToTimestamp(DateTime date)
        {
            long ticks = date.Ticks - DateTime.Parse(EpochTime, CultureInfo.InvariantCulture).Ticks;
            ticks /= TicksPerSecond;
            return (int) ticks;
        }

        private static string EpochTime
        {
            get { return "01/01/1970 00:00:00"; }
        }

        private static int TicksPerSecond
        {
            get { return 10000000; }
        }
    }
}
