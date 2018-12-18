using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Exceptions;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Helpers;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Shared.Repositories;
using BurnForMoney.Functions.Shared.Repositories.Dto;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.External.Strava.Api.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api.Model;
using BurnForMoney.Functions.Strava.Functions.EventsHub.Dto;
using BurnForMoney.Functions.Strava.Security;
using BurnForMoney.Infrastructure.Commands;
using BurnForMoney.Infrastructure.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.EventsHub
{
    public static class EventsRouter
    {
        private static readonly StravaService StravaService = new StravaService();

        [FunctionName(FunctionsNames.EventsRouter)]
        public static async Task EventsHub([QueueTrigger(StravaQueueNames.StravaEvents)] StravaWebhookEvent @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(StravaQueueNames.StravaEventsActivityAdd)] CloudQueue addActivityQueue,
            [Queue(StravaQueueNames.StravaEventsActivityUpdate)] CloudQueue updateActivityQueue,
            [Queue(StravaQueueNames.StravaEventsActivityDelete)] CloudQueue deleteActivityQueue,
            [Queue(StravaQueueNames.DeactivateAthleteRequests)] CloudQueue deauthorizationQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.EventsRouter);

            var stravaAthleteId = @event.OwnerId.ToString();
            
            var readModelFacade = new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var athlete = await readModelFacade.GetAthleteByStravaIdAsync(stravaAthleteId);
            if (athlete == null)
            {
                // This can happen when the athlete is either deactivated (but authenticated) or has not yet been authorized.
                log.LogInformation($"Athlete with strava id: {stravaAthleteId} does not exists.");
                throw new AthleteNotExistsException(null, stravaAthleteId);
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
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {StravaQueueNames.StravaEventsActivityAdd} queue.");
                        await addActivityQueue.AddMessageAsync(new CloudQueueMessage(json));
                        break;
                    case AspectType.Update:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {StravaQueueNames.StravaEventsActivityUpdate} queue.");
                        await updateActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(2), null, null); // Handle quick add->update operation. An event informing about the addition of activity may appear later.
                        break;
                    case AspectType.Delete:
                        log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {StravaQueueNames.StravaEventsActivityDelete} queue.");
                        await deleteActivityQueue.AddMessageAsync(new CloudQueueMessage(json), TimeSpan.FromDays(7), TimeSpan.FromMinutes(2), null, null); // Handle quick add->delete operation. An event informing about the addition of activity may appear later.
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                log.LogInformation(FunctionsNames.EventsRouter, "Event message has been processed.");
            }
            else if (@event.ObjectType == ObjectType.Athlete)
            {
                log.LogInformation(FunctionsNames.EventsRouter, $"Adding message to {StravaQueueNames.DeactivateAthleteRequests} queue.");
                await deauthorizationQueue.AddMessageAsync(new CloudQueueMessage(message.AthleteId.ToString("D")));
            }
            else
            {
                throw new Exception($"Unknown event type: {@event.ObjectType}");
            }
            log.LogFunctionEnd(FunctionsNames.EventsRouter);
        }

        [FunctionName(FunctionsNames.Events_NewActivity)]
        public static async Task Events_NewActivity([QueueTrigger(StravaQueueNames.StravaEventsActivityAdd)] ActivityData activityData,
            ILogger log,
            [Queue(AppQueueNames.AddActivityRequests, Connection = "AppQueuesStorage")] CloudQueue addActivitiesRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_NewActivity);

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
            log.LogFunctionEnd(FunctionsNames.Events_NewActivity);
        }

        [FunctionName(FunctionsNames.Events_UpdateActivity)]
        public static async Task Events_UpdateActivity([QueueTrigger(StravaQueueNames.StravaEventsActivityUpdate)] ActivityData activityData,
            ILogger log,
            [Queue(AppQueueNames.UpdateActivityRequests, Connection = "AppQueuesStorage")] CloudQueue updateActivityRequestsQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_UpdateActivity);

            var activityRepository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activityRow = await activityRepository.GetByExternalIdAsync(activityData.StravaActivityId);

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
            log.LogFunctionEnd(FunctionsNames.Events_UpdateActivity);
        }

        private static async Task<string> GetAccessTokenAsync(Guid athleteId, string keyVaultBaseUrl)
        {
            var secret = await AccessTokensStore.GetAccessTokenForAsync(athleteId, keyVaultBaseUrl);
            return secret.Value;
        }

        [FunctionName(FunctionsNames.Events_DeleteActivity)]
        public static async Task Events_DeleteActivity([QueueTrigger(StravaQueueNames.StravaEventsActivityDelete)] ActivityData @event,
            ILogger log, ExecutionContext executionContext,
            [Queue(AppQueueNames.DeleteActivityRequests, Connection = "AppQueuesStorage")] CloudQueue deleteActivitiesQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.Events_DeleteActivity);

            var activityRepository = new ActivityReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            var activity = await activityRepository.GetByExternalIdAsync(@event.StravaActivityId);

            var json = JsonConvert.SerializeObject(new DeleteActivityCommand { Id = activity.Id, AthleteId = @event.AthleteId });
            var message = new CloudQueueMessage(json);
            await deleteActivitiesQueue.AddMessageAsync(message);
            log.LogFunctionEnd(FunctionsNames.Events_DeleteActivity);
        }
    }
}