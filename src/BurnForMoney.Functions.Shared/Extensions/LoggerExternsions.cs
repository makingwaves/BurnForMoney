using System;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Shared.Extensions
{
    public static class LoggerExternsions
    {
        public static void LogInformation(this ILogger @this, string functionName, string message)
        {
            @this.LogInformation($"[{functionName}] {message}");
        }

        public static void LogWarning(this ILogger @this, string functionName, string message)
        {
            @this.LogWarning($"[{functionName}] {message}");
        }

        public static void LogError(this ILogger @this, string functionName, string message)
        {
            @this.LogWarning($"[{functionName}] {message}");
        }
    }
}