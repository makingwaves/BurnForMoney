namespace BurnForMoney.Infrastructure.Persistence.Repositories.Dto
{
    public class AthleteMonthlyResultActivity
    {
        public string Category { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
        public int Points { get; set; }
        public int NumberOfTrainings { get; set; }
    }
}