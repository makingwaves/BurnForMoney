using System;

namespace BurnForMoney.Functions.Shared.Helpers
{
    public static class UnitsConverter
    {
        public static double ConvertMetersToKilometers(double meters, int digits = 2) => Math.Round(meters / 1000, digits);
        public static double ConvertKilometersToMeters(double kilometers) => kilometers * 1000;
        public static double ConvertSecondsToMinutes(int seconds) => Math.Round(seconds / 60.0, 2);
        public static double ConvertMinutesToHours(double minutes, int digits = 2) => Math.Round(minutes / 60, digits);
        public static double ConvertHoursToMinutes(double hours) => hours * 60;
        public static long ConvertDateTimeToEpoch(DateTime date) => (long)(date - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}