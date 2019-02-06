using System;

namespace BurnForMoney.Identity
{
    public class AthleteIdentity
    {
        public static Guid Next() => Guid.NewGuid();
    }
}