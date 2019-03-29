using System;
using System.Linq;
using System.Security.Claims;

namespace BurnForMoney.ApiGateway.Utils.Extensions
{
    public static class PrincipalHelperExt
    {
        public static Guid GetBfmAthleteId(this ClaimsPrincipal principal)
        {
            return Guid.TryParse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;
        }
    }
}