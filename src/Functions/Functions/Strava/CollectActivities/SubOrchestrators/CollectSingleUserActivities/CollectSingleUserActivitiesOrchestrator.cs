using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Athlete = BurnForMoney.Functions.Persistence.DatabaseSchema.Athlete;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators.CollectSingleUserActivities
{
    public static class CollectSingleUserActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.O_CollectSingleUserActivities)]
        public static async Task O_CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            (Athlete Athlete, bool Optimize) input = context.GetInput<(Athlete, bool)>();

            // 1. Decrypt access token
            var decryptedAccessToken =
                await context.CallActivityAsync<string>(FunctionsNames.A_DecryptAccessToken, input.Athlete.AccessToken);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.O_CollectSingleUserActivities}] Decrypted access token.");
            }

            var getActivitiesFrom = input.Optimize ? input.Athlete.LastUpdate ?? DateTime.UtcNow.AddMonths(-3) : DateTime.UtcNow.AddMonths(-3);

            // 2. Receive and add to queue all new user activities
            await context.CallActivityAsync(FunctionsNames.A_CollectSingleUserActivities, (decryptedAccessToken, getActivitiesFrom));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_CollectSingleUserActivities}] Received and queued all new activities created by: {input.Athlete.FirstName} {input.Athlete.LastName}");
            }

            // 3. Set a new time of the last update
            await context.CallActivityAsync(FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete,
                (AthleteId: input.Athlete.AthleteId, LastUpdate: context.CurrentUtcDateTime));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete}] Updated time of the last update [{context.CurrentUtcDateTime}].");
            }
        }
    }
}