using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Functions
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



        public const string Strava_T_CollectStravaActivitiesInEveryHour = "Strava_T_CollectStravaActivitiesInEveryHour";
        public const string Strava_O_CollectStravaActivities = "Strava_O_CollectStravaActivities";
        public const string Strava_A_GetActiveAthletesWithAccessTokens = "Strava_A_GetActiveAthletesWithAccessTokens";
        public const string Strava_O_CollectSingleUserActivities = "Strava_O_CollectSingleUserActivities";
        public const string Strava_A_CollectSingleUserActivities = "Strava_A_CollectSingleUserActivities";
        public const string A_UpdateLastUpdateDateOfTheUpdatedAthlete = "A_UpdateLastUpdateDateOfTheUpdatedAthlete";


        public const string Q_SubmitAthleteActivity = "Q_SubmitAthleteActivity";

        public const string T_RefreshAccessTokens = "T_RefreshAccessTokens";
        public const string Q_RefreshAccessTokens = "Q_RefreshAccessTokens";
        public const string Q_DeactivateExpiredAccessTokens = "Q_DeactivateExpiredAccessTokens";



        public const string T_CalculateMonthlyAthleteResults = "T_CalculateMonthlyAthleteResults";
        public const string T_CalculateMonthlyAthleteResultsFromPreviousMonth = "T_CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string O_CalculateMonthlyAthleteResults = "O_CalculateMonthlyAthleteResults";
        public const string A_GetActivitiesFromGivenMonth = "A_GetActivitiesFromGivenMonth";
        public const string A_SubmitAthleteMonthlyResults = "A_SubmitAthleteMonthlyResults";




        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_Strava_Activities_Collect = "Support_Strava_Activities_Collect";
        public const string Support_Strava_Activities_CollectMonthlyStatistics = "Support_Strava_Activities_CollectMonthlyStatistics";
        public const string Support_Activities_Add = "Support_Activities_Add";
        public const string Support_Strava_Athlete_Deactivate = "Support_Strava_Athlete_Deactivate";
        public const string Support_Strava_Athlete_Activate = "Support_Strava_Athlete_Activate";

        public const string Support_ReprocessPoisonQueueMessages = "Support_ReprocessPoisonQueueMessages";



    }
}