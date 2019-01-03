using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Strava.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string AuthenticateUser = "AuthenticateUser";

        public const string AuthorizeNewAthleteStarter = "AuthorizeNewAthleteStarter";
        public const string O_AuthorizeNewAthlete = "O_AuthorizeNewAthlete";
        public const string A_GenerateAthleteId = "A_GenerateAthleteId";
        public const string A_ExchangeTokenAndGetAthleteSummary = "A_ExchangeTokenAndGetAthleteSummary";
        public const string A_SendAthleteApprovalRequest = "A_SendAthleteApprovalRequest";
        public const string A_ProcessNewAthleteRequest = "A_ProcessNewAthleteRequest";
        public const string A_AuthorizeNewAthleteCompensation = "A_AuthorizeNewAthleteCompensation";
        public const string SubmitAthleteApproval = "SubmitAthleteApproval";

        public const string Q_CollectAthleteActivities = "Q_CollectAthleteActivities";

        public const string CreateWebhookSubscription = "Webhooks_CreateSubscription";
        public const string WebhooksCallbackValidation = "Webhooks_ValidateCallback";
        public const string ViewWebhookSubscription = "Webhooks_ViewSubscription";
        public const string DeleteWebhookSubscription = "Webhooks_DeleteSubscription";
        public const string EventsHub = "Webhooks_EventsHub";
        public const string EventsRouter = "Webhooks_EventsRouter";
        public const string Events_NewActivity = "Webhooks_Events_NewActivity";
        public const string Events_UpdateActivity = "Webhooks_Events_UpdateActivity";
        public const string Events_DeleteActivity = "Webhooks_Events_DeleteActivity";


        public const string Q_DeactivateAthlete = "Q_DeactivateAthlete";
        public const string Q_ActivateAthlete = "Q_ActivateAthlete";


        public const string T_RefreshAccessTokens = "T_RefreshAccessTokens";
        public const string Q_RefreshAccessTokens = "Q_RefreshAccessTokens";
        public const string Q_DeactivateExpiredAccessTokens = "Q_DeactivateExpiredAccessTokens";
    
        public const string DeactivateAthleteRequests = "strava-deactivate-athlete-requests";
        public const string ActivateAthleteRequests = "strava-activate-athlete-requests";    
    
    
        public const string StravaEvents = "strava-events";
        public const string StravaEventsActivityAdd = "strava-events-activity-add";
        public const string StravaEventsActivityUpdate = "strava-events-activity-update";
        public const string StravaEventsActivityDelete = "strava-events-activity-delete";
    }
}