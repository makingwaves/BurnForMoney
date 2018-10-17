using System;

namespace BurnForMoney.Functions.Persistence.DatabaseSchema
{
    public class Athlete
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool Active { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Source { get; set; }
    }
}