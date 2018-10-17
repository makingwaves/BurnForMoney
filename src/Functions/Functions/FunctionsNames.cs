using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string AuthenticateStravaUser = "AuthenticateStravaUser";

        public const string Strava_AuthorizeNewAthleteStarter = "Strava_AuthorizeNewAthleteStarter";
        public const string Strava_O_AuthorizeNewAthlete = "Strava_O_AuthorizeNewAthlete";
        public const string Strava_A_ExchangeToken = "Strava_A_ExchangeToken";
        public const string Strava_A_EncryptToken = "Strava_A_EncryptToken";
        public const string Strava_A_SendAthleteApprovalRequest = "Strava_A_SendAthleteApprovalRequest";

        public const string Strava_SubmitAthleteApproval = "Strava_SubmitAthleteApproval";

        public const string Strava_Q_ProcessNewAthlete = "Strava_Q_ProcessNewAthlete";
        public const string Q_ProcessRawActivity = "Q_ProcessRawActivity";




        public const string CollectStravaActivitiesInEveryHour = "CollectStravaActivitiesInEveryHour";
        public const string O_CollectStravaActivities = "O_CollectStravaActivities";
        public const string O_CollectSingleUserActivities = "O_CollectSingleUserActivities";
        public const string A_GetAthletesWithAccessTokens = "A_GetAthletesWithAccessTokens";
        public const string A_CollectSingleUserActivities = "A_CollectSingleUserActivities";
        public const string A_UpdateLastUpdateDateOfTheUpdatedAthlete = "A_UpdateLastUpdateDateOfTheUpdatedAthlete";
        public const string A_DecryptAccessToken = "A_DecryptAccessToken";
        public const string Q_SubmitAthleteActivity = "Q_SubmitAthleteActivity";

        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_Strava_Activities_Collect = "Support_Strava_Activities_Collect";
        public const string Support_Strava_Activities_CollectMonthlyStatistics = "Support_Strava_Activities_CollectMonthlyStatistics";
        public const string Support_Activities_Add = "Support_Activities_Add";
        public const string Support_Strava_Athlete_Deactivate = "Support_Strava_Athlete_Deactivate";
        public const string Support_Strava_Athlete_Activate = "Support_Strava_Athlete_Activate";
        
        public const string A_AddAthleteToDatabase = "A_AddAthleteToDatabase";
        
        public const string CalculateMonthlyAthleteResults = "CalculateMonthlyAthleteResults";
        public const string CalculateMonthlyAthleteResultsFromPreviousMonth = "CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string O_CalculateMonthlyAthleteResults = "O_CalculateMonthlyAthleteResults";
        public const string A_GetActivitiesFromGivenMonth = "A_GetActivitiesFromGivenMonth";
        public const string A_GroupActivitiesByAthlete = "A_GroupActivitiesByAthlete";
        public const string A_StoreAggregatedAthleteMonthlyResults = "A_StoreAggregatedAthleteMonthlyResults";
    }
}