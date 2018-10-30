using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Shared.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string AuthenticateStravaUser = "AuthenticateStravaUser";

        public const string Strava_AuthorizeNewAthleteStarter = "Strava_AuthorizeNewAthleteStarter";
        public const string Strava_O_AuthorizeNewAthlete = "Strava_O_AuthorizeNewAthlete";
        public const string Strava_A_ExchangeTokenAndGetAthleteSummary = "Strava_A_ExchangeTokenAndGetAthleteSummary";
        public const string Strava_A_SendAthleteApprovalRequest = "Strava_A_SendAthleteApprovalRequest";
        public const string Strava_SubmitAthleteApproval = "Strava_SubmitAthleteApproval";
        public const string Strava_Q_ProcessNewAthlete = "Strava_Q_ProcessNewAthlete";

        public const string Q_ProcessRawActivity = "Q_ProcessRawActivity";

        public const string Strava_CollectAthleteActivities = "Strava_CollectAthleteActivities";

        public const string Strava_CreateWebhookSubscription = "Strava_Webhooks_CreateSubscription";
        public const string Strava_WebhooksCallbackValidation = "Strava_Webhooks_ValidateCallback";
        public const string Strava_ViewWebhookSubscription = "Strava_Webhooks_ViewSubscription";
        public const string Strava_DeleteWebhookSubscription = "Strava_Webhooks_DeleteSubscription";
        public const string Strava_EventsHub = "Strava_Webhooks_EventsHub";
        public const string Strava_EventsRouter = "Strava_Webhooks_EventsRouter";
        public const string Strava_Events_NewActivity = "Strava_Webhooks_Events_NewActivity";
        public const string Strava_Events_UpdateActivity = "Strava_Webhooks_Events_UpdateActivity";
        public const string Strava_Events_DeleteActivity = "Strava_Webhooks_Events_DeleteActivity";
        public const string Strava_Events_DeauthorizedAthlete = "Strava_Events_DeauthorizedAthlete";


        public const string Q_SubmitAthleteActivity = "Q_SubmitAthleteActivity";

        public const string T_RefreshAccessTokens = "T_RefreshAccessTokens";
        public const string Q_RefreshAccessTokens = "Q_RefreshAccessTokens";
        public const string Q_DeactivateExpiredAccessTokens = "Q_DeactivateExpiredAccessTokens";

        public const string T_CalculateMonthlyAthleteResults = "T_CalculateMonthlyAthleteResults";
        public const string T_CalculateMonthlyAthleteResultsFromPreviousMonth = "T_CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string Q_CalculateMonthlyAthleteResults = "Q_CalculateMonthlyAthleteResults";

        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_Strava_Activities_Collect = "Support_Strava_Activities_Collect";
        public const string Support_Activities_CollectMonthlyStatistics = "Support_Activities_CollectMonthlyStatistics";
        public const string Support_Activities_Add = "Support_Activities_Add";
        public const string Support_Athlete_Deactivate = "Support_Athlete_Deactivate";
        public const string Support_Athlete_Activate = "Support_Athlete_Activate";
        public const string Support_Athlete_Delete = "Support_Athlete_Delete";

        public const string Support_ReprocessPoisonQueueMessages = "Support_ReprocessPoisonQueueMessages";
        public const string Support_PurgeDurableHubHistory = "Support_PurgeDurableHubHistory";
    }
}