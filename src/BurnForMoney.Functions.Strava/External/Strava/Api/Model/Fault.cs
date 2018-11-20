namespace BurnForMoney.Functions.Strava.External.Strava.Api.Model
{
    public class Fault
    {
        public string Message { get; set; }
        public Error[] Errors { get; set; }
    }
}