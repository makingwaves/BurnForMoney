using System;

namespace BurnForMoney.Functions.Strava.Functions.Dto
{
    public class AthleteDto
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}