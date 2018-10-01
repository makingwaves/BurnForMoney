namespace BurnForMoney.Functions.Strava.DatabaseSchema
{
    public class Athlete
    {
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public bool Active { get; set; }
    }
}