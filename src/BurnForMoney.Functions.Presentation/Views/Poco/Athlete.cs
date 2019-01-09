using System;

namespace BurnForMoney.Functions.Presentation.Views.Poco
{
    public class Athlete
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool Active { get; set; }
        public string System { get; set; }
    }
}