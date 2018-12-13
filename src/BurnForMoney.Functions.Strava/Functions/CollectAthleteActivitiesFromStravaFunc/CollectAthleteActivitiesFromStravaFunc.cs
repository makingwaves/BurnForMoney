using System;
using System.Globalization;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.CollectAthleteActivitiesFromStravaFunc
{
    public static class CollectAthleteActivitiesFromStravaFunc
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.Q_CollectAthleteActivities)]
        public static async Task Run([QueueTrigger(StravaQueueNames.CollectAthleteActivities)] CollectStravaActivitiesRequestMessage request,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue addActivitiesRequestsQueue,
            [Queue(StravaQueueNames.UnauthorizedAthletes)] CloudQueue unauthorizedAthletesQueue,
            ILogger log,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Q_CollectAthleteActivities);

            var accessTokenSecret =
                await AccessTokensStore.GetAccessTokenForAsync(request.AthleteId, configuration.Strava.AccessTokensKeyVaultUrl);

            try
            {
                var getActivitiesFrom = request.From ?? GetFirstDayOfTheMonth(DateTime.UtcNow);
                log.LogInformation(FunctionsNames.Q_CollectAthleteActivities, $"Looking for a new activities starting form: {getActivitiesFrom.ToString(CultureInfo.InvariantCulture)}");
                var activities = StravaService.GetActivities(accessTokenSecret.Value, getActivitiesFrom);
                log.LogInformation(FunctionsNames.Q_CollectAthleteActivities, $"Athlete: {request.AthleteId}. Found: {activities.Count} new activities.");

                foreach (var stravaActivity in activities)
                {
                    var command = new AddActivityCommand
                    {
                        Id = ActivityIdentity.Next(),
                        AthleteId = request.AthleteId,
                        ExternalId = stravaActivity.Id.ToString(),
                        ActivityType = stravaActivity.Type.ToString(),
                        StartDate = stravaActivity.StartDate,
                        DistanceInMeters = stravaActivity.Distance,
                        MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(stravaActivity.MovingTime),
                        Source = Source.None
                    };

                    var json = JsonConvert.SerializeObject(command);
                    var message = new CloudQueueMessage(json);
                    await addActivitiesRequestsQueue.AddMessageAsync(message);
                }
            }
            catch (UnauthorizedRequestException ex)
            {
                log.LogError(ex, ex.Message);
                await unauthorizedAthletesQueue.AddMessageAsync(new CloudQueueMessage(request.AthleteId.ToString()));
            }
            log.LogFunctionEnd(FunctionsNames.Q_CollectAthleteActivities);
        }

        private static DateTime GetFirstDayOfTheMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind);
        }
    }
}
