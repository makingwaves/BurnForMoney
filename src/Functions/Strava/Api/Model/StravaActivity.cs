using System;

namespace BurnForMoney.Functions.Strava.Api.Model
{
    public class StravaActivity
    {
        public Athlete Athlete { get; set; }
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public StravaActivityType Type { get; set; }
        public double Distance { get; set; }
        public int MovingTime { get; set; }
    }
}