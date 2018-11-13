using System;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Shared.Extensions
{
    public static class LoggerExternsions
    {
        public static void LogFunctionStart(this ILogger @this, string functionName)
        {
            @this.LogInformation($">>> [{functionName}] function has been started. [{DateTime.UtcNow}]");
        }

        public static void LogFunctionEnd(this ILogger @this, string functionName)
        {
            @this.LogInformation($">>> [{functionName}] function has been completed. [{DateTime.UtcNow}]");
        }

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