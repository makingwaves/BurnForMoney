namespace BurnForMoney.Functions.Helpers
{
    public static class FunctionsNames
    {
        public const string AuthorizeStravaUser = "AuthorizeStravaUser";

        public const string CollectStravaActivitiesInEveryHour = "CollectStravaActivitiesInEveryHour";
        public const string O_CollectStravaActivities = "O_CollectStravaActivities";
        public const string A_GetAccessTokens = "A_GetAccessTokens";
        public const string A_RetrieveAndSaveSingleUserActivities = "A_RetrieveAndSaveSingleUserActivities";
        public const string A_GetLastActivitiesUpdateDate = "A_GetLastActivitiesUpdateDate";
        public const string A_SetLastActivitiesUpdateDate = "A_SetLastActivitiesUpdateDate";
        public const string A_EncryptAccessToken = "A_EncryptAccessToken";
        public const string A_DecryptAccessToken = "A_DecryptAccessToken";


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

        public const string CalculateMonthlyAthleteResults = "CalculateMonthlyAthleteResults";
        public const string CalculateMonthlyAthleteResultsFromPreviousMonth = "CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string O_CalculateMonthlyAthleteResults = "O_CalculateMonthlyAthleteResults";
        public const string A_GetActivitiesFromGivenMonth = "A_GetActivitiesFromGivenMonth";
        public const string A_StoreAggregatedAthleteResults = "A_StoreAggregatedAthleteResults";
    }
}