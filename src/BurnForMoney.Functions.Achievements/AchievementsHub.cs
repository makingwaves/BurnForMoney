using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace BurnForMoney.Functions.Achievements
{
    public static class AchievementsHub
    {
        //[FunctionName("AchievementsHub")]
        //public static void Run([QueueTrigger("domain-events")] DomainEvent @event,
        //    [Table("Achievements")] CloudTable cloudTable,
        //    ILogger log)
        //{
        //    log.LogFunctionStart("AchievementsHub");



        //    var row = new AchievementRow
        //    {
        //        PartitionKey = item.AthleteId,
        //        RowKey = item.AchievementId
        //    };

        //    var tableOperation = TableOperation.Insert(row);



        //    log.LogFunctionEnd("AchievementsHub");
        //}



    }

    public abstract class Achievement
    {
        public string Name { get; }

        public List<ActivityMetric> Metrics { get; set; }

        protected Achievement()
        {
            Name = GetType().Name;
        }

        public abstract bool IsAchieved();
    }

    public class Run1000Kilometers : Achievement
    {
        public double Kilometers { get; set; }

        public override bool IsAchieved()
        {
            return Kilometers >= 1000;
        }
    }

    public class ActivityMetric
    {
        public string ActivityId { get; set; }
    }





    public class AchievementEvent
    {
        public string AthleteId { get; set; }
        public string AchievementId { get; set; }
    }

    public class AchievementEntity : TableEntity
    {

    }

    public class AchievementRow : AchievementEntity
    {
    }

}