namespace Infrastructure.Services.Notifications
{
    internal static class NepaliDateTimeHelper
    {
        private static readonly TimeSpan NepalOffset = new(5, 45, 0);
        private static readonly DateOnly AnchorAd = new(2026, 4, 14);
        private static readonly int[] MonthLengths2083 = [31, 32, 31, 32, 31, 30, 30, 30, 29, 30, 30, 30];

        public static string FromUtc(DateTime utcDateTime)
        {
            var nepalDateTime = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime.Add(NepalOffset)
                : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).Add(NepalOffset);

            var adDate = DateOnly.FromDateTime(nepalDateTime);
            var days = adDate.DayNumber - AnchorAd.DayNumber;

            if (days < 0 || days >= MonthLengths2083.Sum())
            {
                return $"{nepalDateTime:yyyy-MM-dd HH:mm}";
            }

            var month = 1;
            var day = days + 1;
            foreach (var monthLength in MonthLengths2083)
            {
                if (day <= monthLength)
                {
                    break;
                }

                day -= monthLength;
                month++;
            }

            return $"2083-{month:00}-{day:00} {nepalDateTime:HH:mm}";
        }

        public static string FromBsDateAndUtcTime(string bsDate, DateTime utcDateTime)
        {
            var nepalDateTime = utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime.Add(NepalOffset)
                : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).Add(NepalOffset);

            return $"{bsDate.Trim()} {nepalDateTime:HH:mm}";
        }
    }
}
