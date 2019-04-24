using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Infrastructure.Queues;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Strava.Commands;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Functions.EventsHub.Dto;
using BurnForMoney.Functions.Strava.Security;
using BurnForMoney.Identity;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class EventsRouter
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.EventsRouter)]
        public static async Task EventsHub([QueueTrigger(QueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(QueueNames.StravaEventsActivityAdd)] CloudQueue addActivityQueue,
            [Queue(QueueNames.StravaEventsActivityUpdate)] CloudQueue updateActivityQueue,
            [Queue(QueueNames.StravaEventsActivityDelete)] CloudQueue deleteActivityQueue,
            [Queue(QueueNames.DeactivateAthleteRequests)] CloudQueue deauthorizationQueue,
            [Inject] IAthleteReadRepository repository,
            [Configuration] ConfigurationRoot configuration)
        {
            var stravaAthleteId = @event.OwnerId.ToString();
            var athlete = await repository.GetAthleteByStravaIdAsync(stravaAthleteId);
            if (athlete == null)
            {
                // This can happen when the athlete is either deactivated (but authenticated) or has not yet been authorized.
                log.LogWarning($"Athlete with strava id: {stravaAthleteId} does not exists.");
                return;
            }

            if (athlete == AthleteRow.NonActive)
            {
                log.LogInformation($"Received an event from inactive athlete [{athlete.Id}].");
                return;
            }

            var message = new ActivityData(athlete.Id, @event.ObjectId.ToString());
            var json = JsonConvert.SerializeObject(message);

            if (@event.ObjectType == ObjectType.Activity)
            {
                switch (@event.AspectType)
                {
                    case AspectType.Create:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityAdd} queue.");
                        await addActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Update:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityUpdate} queue.");
                        await updateActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(2), null, null); // Handle quick add->update operation. An event informing about the addition of activity may appear later.
                        break;
                    case AspectType.Delete:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.StravaEventsActivityDelete} queue.");
                        await deleteActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(2), null, null); // Handle quick add->delete operation. An event informing about the addition of activity may appear later.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                log.LogInformation(FunctionsNames.EventsRouter, "Event message has been processed.");
            }
            else if (@event.ObjectType == ObjectType.Athlete)
            {
                log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {QueueNames.DeactivateAthleteRequests} queue.");
                await deauthorizationQueue.AddMessageAsync(new CloudQueueMessage(message.AthleteId.ToString("D")));
            }
            else
            {
                throw new Exception($"Unknown event type: {@event.ObjectType}");
            }
        }

        [FunctionName(FunctionsNames.Events_NewActivity)]
        public static async Task Events_NewActivity([QueueTrigger(QueueNames.StravaEventsActivityAdd)] ActivityData activityData,
            ILogger log,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue addActivitiesRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var accessToken = await GetAccessTokenAsync(activityData.AthleteId, configuration.Strava.AccessTokensKeyVaultUrl);

            StravaActivity activity;
            try
            {
                activity = StravaService.GetActivity(accessToken, activityData.StravaActivityId);
            }
            catch (ActivityNotFoundException ex)
            {
                log.LogWarning(ex.Message);
                return;
            }

            var command = new AddActivityCommand
            {
                Id = ActivityIdentity.Next(),
                ExternalId = activity.Id.ToString(),
                AthleteId = activityData.AthleteId,
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime),
                Source = Source.Strava
            };

            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await addActivitiesRequestsQueue.AddMessageAsync(message);
        }

        [FunctionName(FunctionsNames.Events_UpdateActivity)]
        public static async Task Events_UpdateActivity([QueueTrigger(QueueNames.StravaEventsActivityUpdate)] ActivityData activityData,
            ILogger log,
            [Queue(AppQueueNames.UpdateActivityRequests, Connection = "AppQueuesStorage")] CloudQueue updateActivityRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var activityRepository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);

            var accessToken = await GetAccessTokenAsync(activityData.AthleteId, configuration.Strava.AccessTokensKeyVaultUrl);
            StravaActivity activity;
            try
            {
                activity = StravaService.GetActivity(accessToken, activityData.StravaActivityId);
            }
            catch (ActivityNotFoundException ex)
            {
                log.LogWarning(ex.Message);
                return;
            }

            var activityRow = await activityRepository.GetByExternalIdAsync(activityData.StravaActivityId);
            var command = new UpdateActivityCommand
            {
                Id = activityRow.Id,
                AthleteId = activityData.AthleteId,
                ActivityType = activity.Type.ToString(),
                StartDate = activity.StartDate,
                DistanceInMeters = activity.Distance,
                MovingTimeInMinutes = UnitsConverter.ConvertSecondsToMinutes(activity.MovingTime)
            };

            var json = JsonConvert.SerializeObject(command);
            var message = new CloudQueueMessage(json);
            await updateActivityRequestsQueue.AddMessageAsync(message);
        }

        private static async Task<string> GetAccessTokenAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            var secret = await AccessTokensStore.GetAccessTokenForAsync(athleteId, keyVaultBaseUrl);
            return secret.Value;
        }

        [FunctionName(FunctionsNames.Events_DeleteActivity)]
        public static async Task Events_DeleteActivity([QueueTrigger(QueueNames.StravaEventsActivityDelete)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue deleteActivitiesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            var activityRepository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activity = await activityRepository.GetByExternalIdAsync(@event.StravaActivityId);

            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = activity.Id, AthleteId = @event.AthleteId });
            var message = new CloudQueueMessage(json);
            await deleteActivitiesQueue.AddMessageAsync(message);
        }
    }
}