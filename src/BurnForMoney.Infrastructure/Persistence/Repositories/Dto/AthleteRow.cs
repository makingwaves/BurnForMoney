using System;

namespace BurnForMoney.Infrastructure.Persistence.Repositories.Dto
{
    public static class AthleteRowExt
    {
        public static bool IsValid(this AthleteRow row)
        {
            return row != null && row != AthleteRow.NonActive;
        }
    }

    public class AthleteRow
    {
        public static readonly AthleteRow NonActive = new AthleteRow();

        public Guid Id { get; set; }
        public Guid ActiveDirectoryId { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string System { get; set; }
        public bool Active { get; set; }
    }
}