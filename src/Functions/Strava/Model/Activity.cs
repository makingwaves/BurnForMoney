using System;

namespace BurnForMoney.Functions.Strava.Model
{
    public class Activity
    {
        public Athlete Athlete { get; set; }
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public string Type { get; set; }
        public double Distance { get; set; }
        public int MovingTime { get; set; }
    }
}