namespace BurnForMoney.Functions.Shared.Repositories.Dto
{
    public class AthleteRow : Row
    {
        public static readonly AthleteRow NonActive = new AthleteRow();

        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string System { get; set; }
        public bool Active { get; set; }
    }
}