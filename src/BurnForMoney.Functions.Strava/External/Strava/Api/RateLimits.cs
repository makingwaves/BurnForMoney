namespace BurnForMoney.Functions.Strava.External.Strava.Api
{
    public class RateLimits
    {
        public int ShortTermLimit { get; set; }
        public int DailyLimit { get; set; }
        public int ShortTermUsage { get; set; }
        public int DailyUsage { get; set; }

        public bool IsUsageWithinTheLimits()
        {
            return ShortTermUsage < ShortTermLimit && DailyUsage < DailyLimit;
        }
    }
}