using System;

namespace BurnForMoney.Identity
{
    public class ActivityIdentity
    {
        public static Guid Next() => Guid.NewGuid();
    }
}