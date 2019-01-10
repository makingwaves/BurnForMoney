using System.Diagnostics.CodeAnalysis;
using BurnForMoney.Functions.Shared;

namespace BurnForMoney.Functions.Strava.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string HttpTriggerPrefix = FunctionNameConvention.HttpTriggerPrefix;
        public const string QueueTriggerPrefix = FunctionNameConvention.QueueTriggerPrefix;
        public const string TimerTriggerPrefix = FunctionNameConvention.TimerTriggerPrefix;
        public const string DurableActivityPrefix = FunctionNameConvention.DurableActivityPrefix;
        public const string DurableOrchestratorPrefix = FunctionNameConvention.DurableOrchestratorPrefix;

        public const string AuthenticateUser = HttpTriggerPrefix + "AuthenticateUser";
        public const string AuthorizeNewAthleteStarter = HttpTriggerPrefix + "AuthorizeNewAthleteStarter";
        public const string O_AuthorizeNewAthlete = DurableOrchestratorPrefix + "AuthorizeNewAthlete";
        public const string A_GenerateAthleteId = DurableActivityPrefix + "GenerateAthleteId";
        public const string A_ExchangeTokenAndGetAthleteSummary = DurableActivityPrefix + "ExchangeTokenAndGetAthleteSummary";
        public const string A_SendAthleteApprovalRequest = DurableActivityPrefix + "SendAthleteApprovalRequest";
        public const string A_ProcessNewAthleteRequest = DurableActivityPrefix + "ProcessNewAthleteRequest";
        public const string A_AuthorizeNewAthleteCompensation = DurableActivityPrefix + "AuthorizeNewAthleteCompensation";
        public const string SubmitAthleteApproval = HttpTriggerPrefix + "SubmitAthleteApproval";

        public const string Q_CollectAthleteActivities = QueueTriggerPrefix + "CollectAthleteActivities";

        public const string CreateWebhookSubscription = "Webhooks_CreateSubscription";
        public const string WebhooksCallbackValidation = "Webhooks_ValidateCallback";
        public const string ViewWebhookSubscription = "Webhooks_ViewSubscription";
        public const string DeleteWebhookSubscription = "Webhooks_DeleteSubscription";
        public const string EventsHub = "Webhooks_EventsHub";
        public const string EventsRouter = "Webhooks_EventsRouter";
        public const string Events_NewActivity = "Webhooks_Events_NewActivity";
        public const string Events_UpdateActivity = "Webhooks_Events_UpdateActivity";
        public const string Events_DeleteActivity = "Webhooks_Events_DeleteActivity";


        public const string Q_DeactivateAthlete = QueueTriggerPrefix + "DeactivateAthlete";
        public const string Q_ActivateAthlete = QueueTriggerPrefix + "ActivateAthlete";


        public const string T_RefreshAccessTokens = TimerTriggerPrefix + "RefreshAccessTokens";
        public const string Q_RefreshAccessTokens = QueueTriggerPrefix + "RefreshAccessTokens";
        public const string Q_DeactivateExpiredAccessTokens = QueueTriggerPrefix + "DeactivateExpiredAccessTokens";}
}