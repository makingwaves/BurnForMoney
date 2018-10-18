using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Functions.Strava.CollectActivities.SubOrchestrators.CollectSingleUserActivities
{
    public static class CollectSingleUserActivitiesOrchestrator
    {
        [FunctionName(FunctionsNames.Strava_O_CollectSingleUserActivities)]
        public static async Task O_CollectStravaActivitiesAsync(ILogger log, [OrchestrationTrigger] DurableOrchestrationContext context, ExecutionContext executionContext)
        {
            (AthleteWithAccessToken Athlete, bool Optimize) input = context.GetInput<(AthleteWithAccessToken, bool)>();

            // 1. Decrypt access token
            var decryptedAccessToken =
                await context.CallActivityAsync<string>(FunctionsNames.Strava_A_DecryptAccessToken, input.Athlete.EncryptedAccessToken);
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.Strava_O_CollectSingleUserActivities}] Decrypted access token.");
            }
            
            var getActivitiesFrom = input.Optimize ? input.Athlete.LastUpdate ?? GetFirstDayOfTheMonth(DateTime.UtcNow) : GetFirstDayOfTheMonth(DateTime.UtcNow);

            // 2. Receive and add to queue all new user activities
            await context.CallActivityAsync(FunctionsNames.Strava_A_CollectSingleUserActivities, (input.Athlete.Id, decryptedAccessToken, getActivitiesFrom));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.Strava_A_CollectSingleUserActivities}] Received and queued all new activities created by athlete with id: {input.Athlete.Id}.");
            }

            // 3. Set a new time of the last update
            await context.CallActivityAsync(FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete,
                (AthleteId: input.Athlete.Id, LastUpdate: context.CurrentUtcDateTime));
            if (!context.IsReplaying)
            {
                log.LogInformation($"[{FunctionsNames.A_UpdateLastUpdateDateOfTheUpdatedAthlete}] Updated time of the last update [{context.CurrentUtcDateTime}].");
            }
        }

        private static DateTime GetFirstDayOfTheMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }
    }
}