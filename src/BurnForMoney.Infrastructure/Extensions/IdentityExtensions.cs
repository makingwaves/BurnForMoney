using System;

namespace BurnForMoney.Infrastructure.Extensions
{
    public static class IdentityExtensions
    {
        public static string ToUpperInvariant(this Guid id)
        {
            return id.ToString("D").ToUpperInvariant();
        }
    }
}
