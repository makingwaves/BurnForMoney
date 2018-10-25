using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Functions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities
{
    public static class CollectActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.Strava_O_CollectStravaActivities)]
        public static async Task O_CollectStravaActivitiesAsync(ILogger log,
            [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            var optimize = context.GetInput<bool?>() ?? true;

            if (!context.IsReplaying)
            {
                log.LogInformation(
                    $"Orchestration function `{FunctionsNames.Strava_O_CollectStravaActivities}` received a request. {{optimize}}: {optimize}.");
            }

            // 1. Get active athletes
            var athletes = await context.CallActivityAsync<AthleteWithAccessToken[]>(FunctionsNames.Strava_A_GetActiveAthletesWithAccessTokens,
                    ActivityInput.Empty);
            if (athletes.Length == 0)
            {
                log.LogInformation($"[{FunctionsNames.Strava_O_CollectStravaActivities}] cannot find any active athletes.");
                return;
            }
            if (!context.IsReplaying)
            {
                log.LogInformation(
                    $"[{FunctionsNames.Strava_O_CollectStravaActivities}] Retrieved data about {athletes.Length} active athletes.");
            }

            // 2. Collect activities of the all athletes
            var tasks = new Task[athletes.Length];
            for (var i = 0; i < athletes.Length; i++)
            {
                tasks[i] = context.CallSubOrchestratorAsync(FunctionsNames.Strava_O_CollectSingleUserActivities, (athletes[i], optimize));
            }

            await Task.WhenAll(tasks);
        }
    }
}
