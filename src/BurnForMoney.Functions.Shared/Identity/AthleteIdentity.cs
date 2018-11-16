using System;

namespace BurnForMoney.Functions.Shared.Identity
{
    public class AthleteIdentity
    {
        public static string Next() => Guid.NewGuid().ToString("N");
    }
}