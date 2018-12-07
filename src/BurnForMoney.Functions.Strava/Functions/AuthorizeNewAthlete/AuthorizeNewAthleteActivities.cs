using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Functions.Shared.Identity;
using BurnForMoney.Functions.Shared.Queues;
using BurnForMoney.Functions.Strava.Configuration;
using BurnForMoney.Functions.Strava.Exceptions;
using BurnForMoney.Functions.Strava.External.Strava.Api;
using BurnForMoney.Functions.Strava.Functions.AddNewAthlete;
using BurnForMoney.Functions.Strava.Functions.Dto;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Strava.Functions.AuthorizeNewAthlete
{
    public static class AuthorizeNewAthleteActivities
    {
        private static readonly StravaService StravaService = new StravaService();
        
        [FunctionName(FunctionsNames.A_GenerateAthleteId)]
        public static string A_GenerateAthleteId([ActivityTrigger] object input)
        {
            return AthleteIdentity.Next();
        }

        [FunctionName(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary)]
        public static async Task<Athlete> A_ExchangeTokenAndGetAthleteSummaryAsync([ActivityTrigger]A_ExchangeTokenAndGetAthleteSummaryInput input, ILogger log,
            [Table("Athletes", Connection = "AppStorage")] CloudTable athletesTable,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary);

            log.LogInformation($"Requesting for access token using clientId: {configuration.Strava.ClientId}.");
            var response = StravaService.ExchangeToken(configuration.Strava.ClientId, configuration.Strava.ClientSecret, input.AuthorizationCode);

            var query = new TableQuery<AthleteEntity>
            {
                FilterString =
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, response.Athlete.Id.ToString())
            };
            var queryResult = await athletesTable.ExecuteQuerySegmentedAsync(query, null);
            if (queryResult.Any())
            {
                throw new AthleteAlreadyExistsException(response.Athlete.Id.ToString());
            }

            try
            {
                await AccessTokensStore.AddAsync(input.AthleteId, response.AccessToken, response.RefreshToken, response.ExpiresAt,
                    configuration.Strava.AccessTokensKeyVaultUrl);

                log.LogInformation(nameof(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary), $"Updated tokens for athlete with id: {configuration.Strava.ClientId}.");
            }
            catch (Exception ex)
            {
                throw new FailedToAddAccessTokenException(response.Athlete.Id.ToString(), ex);
            }

            log.LogFunctionEnd(FunctionsNames.A_ExchangeTokenAndGetAthleteSummary);
            return new Athlete
            {
                Id = input.AthleteId,
                ExternalId = response.Athlete.Id.ToString(),
                FirstName = response.Athlete.Firstname,
                LastName = response.Athlete.Lastname,
                ProfilePictureUrl = response.Athlete.Profile
            };
        }

        [FunctionName(FunctionsNames.A_SendAthleteApprovalRequest)]
        public static async Task A_SendAthleteApprovalRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            [Queue(AppQueueNames.NotificationsToSend, Connection = "AppQueuesStorage")] CloudQueue notificationsQueue,
            [Table("AthleteApprovals", "AzureWebJobsStorage")] IAsyncCollector<AthleteApproval> athleteApprovalCollector,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.A_SendAthleteApprovalRequest);
            var (firstName, lastName) = activityContext.GetInput<(string, string)>();

            var approvalCode = Guid.NewGuid().ToString("N");
            var athleteApproval = new AthleteApproval
            {
                PartitionKey = "AthleteApproval",
                RowKey = approvalCode,
                OrchestrationId = activityContext.InstanceId
            };

            var approvalFunctionAddress = $"{configuration.HostName}/api/SubmitAthleteApproval/{approvalCode}";
            var notification = new Notification
            {
                Recipients = new List<string> { configuration.Email.AthletesApprovalEmail },
                Subject = "Athlete is awaiting approval",
                HtmlContent = $@"
<p>Hi there,</p>
<p>Please review a new authorization request. Athlete: {firstName} {lastName}.</p>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Approved.ToString()}\">Approve</a><br>" +
                          $"<a href=\"{approvalFunctionAddress}?result={AthleteApprovalResult.Rejected.ToString()}\">Reject</a>"
            };

            log.LogInformation(FunctionsNames.A_SendAthleteApprovalRequest, $"Sending approval request for athlete {firstName} {lastName} to: {configuration.Email.AthletesApprovalEmail}.");
            await athleteApprovalCollector.AddAsync(athleteApproval);
            await notificationsQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(notification)));

            log.LogFunctionEnd(FunctionsNames.A_SendAthleteApprovalRequest);
        }

        [FunctionName(FunctionsNames.A_ProcessNewAthleteRequest)]
        public static async Task A_ProcessNewAthleteRequest([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(QueueNames.AddStravaAthleteRequests)] CloudQueue addAthleteRequestsQueue)
        {
            log.LogFunctionStart(FunctionsNames.A_ProcessNewAthleteRequest);

            var athlete = activityContext.GetInput<Athlete>();
            var json = JsonConvert.SerializeObject(athlete);
            await addAthleteRequestsQueue.AddMessageAsync(new CloudQueueMessage(json));

            log.LogFunctionEnd(FunctionsNames.A_ProcessNewAthleteRequest);
        }

        [FunctionName(FunctionsNames.A_AuthorizeNewAthleteCompensation)]
        public static async Task A_AuthorizeNewAthleteCompensation([ActivityTrigger]DurableActivityContext activityContext, ILogger log,
            ExecutionContext context, [Queue(QueueNames.AuthorizationCodesPoison)] CloudQueue authorizationCodePoisonQueue,
            [Configuration] ConfigurationRoot configuration)
        {
            log.LogFunctionStart(FunctionsNames.A_AuthorizeNewAthleteCompensation);

            var input = activityContext.GetInput<AuthorizeNewAthleteCompensation>();
            var json = JsonConvert.SerializeObject(input);
            await authorizationCodePoisonQueue.AddMessageAsync(new CloudQueueMessage(json));

            try
            {
                await AccessTokensStore.DeleteAsync(input.AthleteId, configuration.Strava.AccessTokensKeyVaultUrl);
            }
            catch (KeyVaultErrorException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                // ignored, it may not be added
            }

            log.LogFunctionEnd(FunctionsNames.A_AuthorizeNewAthleteCompensation);
        }
    }

    internal enum AthleteApprovalResult
    {
        Approved,
        Rejected
    }

    public class AthleteApproval
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrchestrationId { get; set; }
    }

    public class A_ExchangeTokenAndGetAthleteSummaryInput
    {
        public Guid AthleteId { get; set; }
        public string AuthorizationCode { get; set; }

        public A_ExchangeTokenAndGetAthleteSummaryInput(Guid athleteId, string authorizationCode)
        {
            AthleteId = athleteId;
            AuthorizationCode = authorizationCode;
        }
    }
}
