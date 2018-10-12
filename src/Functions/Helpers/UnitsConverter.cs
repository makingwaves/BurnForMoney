using System;

namespace BurnForMoney.Functions.Helpers
{
    public static class UnitsConverter
    {
        public static double ConvertMetersToKilometers(double meters, int digits = 2) => Math.Round(meters / 1000, digits);
        public static double ConvertMinutesToHours(double minutes, int digits = 2) => Math.Round(minutes / 60, digits);
        public static long ConvertDateTimeToEpoch(DateTime date) => (long)(date - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}