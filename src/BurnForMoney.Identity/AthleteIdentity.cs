using System;

namespace BurnForMoney.Identity
{
    public class AthleteIdentity
    {
        public static string Next() => Guid.NewGuid().ToString("N");
    }
}