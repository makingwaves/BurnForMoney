using System;

namespace BurnForMoney.Functions.Shared.Identity
{
    public class ActivityIdentity
    {
        public static Guid Next() => Guid.NewGuid();
    }
}