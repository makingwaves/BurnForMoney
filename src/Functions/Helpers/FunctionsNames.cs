namespace BurnForMoney.Functions.Helpers
{
    public static class FunctionsNames
    {
        public const string AuthenticateStravaUser = "AuthenticateStravaUser";

        public const string CollectStravaActivitiesInEveryHour = "CollectStravaActivitiesInEveryHour";
        public const string O_CollectStravaActivities = "O_CollectStravaActivities";
        public const string O_CollectSingleUserActivities = "O_CollectSingleUserActivities";
        public const string O_DecryptAllAccessTokens = "O_DecryptAllAccessTokens";
        public const string O_RetrieveAllStravaActivities = "O_RetrieveAllStravaActivities";
        public const string A_GetAthletesWithAccessTokens = "A_GetAthletesWithAccessTokens";
        public const string A_RetrieveSingleUserActivities = "A_RetrieveSingleUserActivities";
        public const string A_UpdateLastUpdateDateOfTheUpdatedAthlete = "A_UpdateLastUpdateDateOfTheUpdatedAthlete";
        public const string A_ProcessSingleUserActivities = "A_ProcessSingleUserActivities";
        public const string A_EncryptAccessToken = "A_EncryptAccessToken";
        public const string A_DecryptAccessToken = "A_DecryptAccessToken";
        public const string Q_SubmitAthleteActivity = "Q_SubmitAthleteActivity";


        public const string Support_EncryptString = "Support_EncryptString";
        public const string Support_DecryptString = "Support_DecryptString";
        public const string Support_InitializeDatabase = "Support_InitializeDatabase";
        public const string Support_Strava_Activities_Collect = "Support_Strava_Activities_Collect";
        public const string Support_Strava_Activities_CollectMonthlyStatistics = "Support_Strava_Activities_CollectMonthlyStatistics";
        public const string Support_Strava_Athlete_Deactivate = "Support_Strava_Athlete_Deactivate";
        public const string Support_Strava_Athlete_Activate = "Support_Strava_Athlete_Activate";
        
        public const string AuthorizeNewAthleteStarter = "AuthorizeNewAthleteStarter";
        public const string O_AuthorizeNewAthlete = "O_AuthorizeNewAthlete";
        public const string A_GenerateAccessToken = "A_GenerateAccessToken";
        public const string A_AddAthleteToDatabase = "A_AddAthleteToDatabase";
        public const string A_SendAthleteApprovalRequest = "A_SendAthleteApprovalRequest";
        public const string SubmitAthleteApproval = "SubmitAthleteApproval";
        
        public const string CalculateMonthlyAthleteResults = "CalculateMonthlyAthleteResults";
        public const string CalculateMonthlyAthleteResultsFromPreviousMonth = "CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string O_CalculateMonthlyAthleteResults = "O_CalculateMonthlyAthleteResults";
        public const string A_GetActivitiesFromGivenMonth = "A_GetActivitiesFromGivenMonth";
        public const string A_GroupActivitiesByAthlete = "A_GroupActivitiesByAthlete";
        public const string A_StoreAggregatedAthleteMonthlyResults = "A_StoreAggregatedAthleteMonthlyResults";
    }
}