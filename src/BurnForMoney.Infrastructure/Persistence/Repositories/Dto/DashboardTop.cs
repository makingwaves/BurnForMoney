namespace BurnForMoney.Infrastructure.Persistence.Repositories.Dto
{
    public class DashboardTop
    {
        public double TotalDistance { get; set; }
        public double TotalTime { get; set; }
        public int TotalPoints { get; set; }
        public decimal TotalMoney { get; set; }
        public int CurrentPoints { get; set; }
        public int PointsThreshold { get; set; }
        public decimal Payment { get; set; }
    }
}