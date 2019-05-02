using System;
using System.Collections.Generic;
using System.Text;

namespace BurnForMoney.Functions.PublicApi.Calculators
{
    public class TotalNumbers
    {
        public int Distance { get; set; }
        public int Time { get; set; }
        public int Money { get; set; }
        public ThisMonth ThisMonth { get; set; }
    }

    public class ThisMonth
    {
        public static ThisMonth NoResults = new ThisMonth
        {
            NumberOfTrainings = 0,
            PercentOfEngagedEmployees = 0,
            Points = 0,
            Money = 0,
            MostFrequentActivities = new List<object>()
        };

        public int NumberOfTrainings { get; set; }
        public int PercentOfEngagedEmployees { get; set; }
        public int Points { get; set; }
        public int Money { get; set; }
        public IEnumerable<object> MostFrequentActivities { get; set; }
    }

    public class DateComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            var xSplit = x.Split('/');
            var xDate = new DateTime(int.Parse(xSplit[0]), int.Parse(xSplit[1]), 1, 0, 0, 0);
            var ySplit = y.Split('/');
            var yDate = new DateTime(int.Parse(ySplit[0]), int.Parse(ySplit[1]), 1, 0, 0, 0);
            return xDate.CompareTo(yDate);
        }
    }
}
