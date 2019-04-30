using System;

namespace BurnForMoney.Functions.InternalApi.Functions.Athletes.Dto
{
    public class CreateAthleteRequest
    {
        public Guid AadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void Validate()
        {
            if (AadId == Guid.Empty)
            {
                throw new ArgumentException(nameof(AadId));
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                throw new ArgumentNullException(nameof(FirstName));
            }
        }
    }
}