using System;

namespace BurnForMoney.Functions.InternalApi.Functions.Activities.Dto
{
    public class ActivityAddOrUpdateRequest
    {
        public DateTime? StartDate { get; set; }
        public string Type { get; set; }
        public double? DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }

        public void Validate()
        {
            if (StartDate == null)
            {
                throw new ArgumentNullException(nameof(StartDate));
            }
            if (string.IsNullOrWhiteSpace(Type))
            {
                throw new ArgumentNullException(nameof(Type));
            }
            if (MovingTimeInMinutes <= 0)
            {
                throw new ArgumentNullException(nameof(MovingTimeInMinutes));
            }

            this.ValidateValues();
        }

        private void ValidateValues()
        {
            var minStartDate = DateTime.UtcNow.AddDays(-40);
            if (StartDate.Value < minStartDate)
            {
                throw new ArgumentOutOfRangeException($"[StartDate] StartDate should starts after: {minStartDate.ToString()}.");
            }
            if (StartDate.Value > DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException("[StartDate] Cannot add future start date.");
            }
            var activityEnd = StartDate.Value.AddMinutes(MovingTimeInMinutes);
            if (activityEnd > DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException($"[StartDate] Activity cannot end in the future ({activityEnd}).");
            }
            if (DistanceInMeters > 1000 * 1000)
            {
                throw new ArgumentOutOfRangeException("[DistanceInMeters] Activity distance cannot be higher than 1000 kilometers");
            }
            if (MovingTimeInMinutes > 24 * 60)
            {
                throw new ArgumentOutOfRangeException("[MovingTimeInMinutes] Activity moving time cannot be longer than 24 hours.");
            }
        }
    }
}