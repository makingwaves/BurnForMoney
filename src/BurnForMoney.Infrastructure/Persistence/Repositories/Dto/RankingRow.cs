using System;

namespace BurnForMoney.Infrastructure.Persistence.Repositories.Dto
{
    public class RankingByPoints
    {

        public Guid AthleteId { get; set; }
        public string AthleteFirstName { get; set; }
        public string AthleteLastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        
        public double Points { get; set; }
    }
}