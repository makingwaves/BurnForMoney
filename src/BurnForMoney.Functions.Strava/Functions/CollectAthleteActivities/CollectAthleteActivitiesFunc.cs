using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using BurnForMoney.Functions.Strava.Functions.CollectAthleteActivities.Dto;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.CollectAthleteActivities
{
    public static class CollectAthleteActivitiesFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Q_CollectAthleteActivities)]
        public static async Task Run([QueueTrigger(QueueNames.CollectAthleteActivities)] CollectAthleteActivitiesInput input,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue pendingRawActivitiesQueue,
            [Queue(QueueNames.UnauthorizedAccessTokens)] CloudQueue unauthorizedAccessTokensQueue,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogFunctionStart(FunctionsNames.Q_CollectAthleteActivities);

            var configuration = ApplicationConfiguration.GetSettings(executionContext);

            string encryptedAccessToken;
            string accessToken;
            using (var conn = new SqlConnection(configuration.ConnectionStrings.SqlDbConnectionString))
            {
                encryptedAccessToken = await conn.QuerySingleOrDefaultAsync<string>(@"SELECT AccessToken
FROM dbo.[Strava.AccessTokens]
WHERE AthleteId = @AthleteId AND IsValid=1", new { input.AthleteId });

                if (string.IsNullOrWhiteSpace(encryptedAccessToken))
                {
                    throw new AccessTokenNotFoundException(input.AthleteId);
                }

                accessToken = AccessTokensEncryptionService.Decrypt(encryptedAccessToken,
                    configuration.Strava.AccessTokensEncryptionKey);
                log.LogInformation(FunctionsNames.Q_CollectAthleteActivities, "Decrypted access token.");
            }

            try
            {
                var getActivitiesFrom = input.From ?? GetFirstDayOfTheMonth(DateTime.UtcNow);
                log.LogInformation(FunctionsNames.Q_CollectAthleteActivities, $"Looking for a new activities starting form: {getActivitiesFrom.ToString(CultureInfo.InvariantCulture)}");
                var activities = StravaService.GetActivities(accessToken, getActivitiesFrom);
                log.LogInformation(FunctionsNames.Q_CollectAthleteActivities, $"Athlete: {input.AthleteId}. Found: {activities.Count} new activities.");
                foreach (var stravaActivity in activities)
                {
                    var pendingActivity = new PendingRawActivity
                    {
                        Id = ActivityIdentity.Next(),
                        ExternalId = stravaActivity.Id.ToString(),
                        ExternalAthleteId = stravaActivity.Athlete.Id.ToString(),
                        ActivityType = stravaActivity.Type.ToString(),
                        StartDate = stravaActivity.StartDate,
                        DistanceInMeters = stravaActivity.Distance,
                        MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(stravaActivity.MovingTime),
                        Source = "Strava"
                    };

                    var json = JsonConvert.SerializeObject(pendingActivity);
                    var message = new CloudQueueMessage(json);
                    await pendingRawActivitiesQueue.AddMessageAsync(message);
                }
            }
            catch (UnauthorizedRequestException ex)
            {
                log.LogError(ex, ex.Message);
                await unauthorizedAccessTokensQueue.AddMessageAsync(new CloudQueueMessage(encryptedAccessToken));
            }
            log.LogFunctionEnd(FunctionsNames.Q_CollectAthleteActivities);
        }

        private static DateTime GetFirstDayOfTheMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }
    }
}
