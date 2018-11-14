using System;

namespace BurnForMoney.Functions.Shared.Identity
{
    public class ActivityIdentity
    {
        public static string Next() => Guid.NewGuid().ToString("N");
    }
}