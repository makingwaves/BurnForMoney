using System;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete.Dto
{
    public class StravaAthlete
    {
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }

        public string EncryptedAccessToken { get; set; }
        public string EncryptedRefreshToken { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}