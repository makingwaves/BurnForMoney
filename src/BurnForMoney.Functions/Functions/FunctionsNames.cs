using System.Diagnostics.CodeAnalysis;

namespace BurnForMoney.Functions.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FunctionsNames
    {
        public const string Q_AddAthlete = "Q_AddAthlete";
        public const string Q_DeactivateAthlete = "Q_DeactivateAthlete";
        public const string Q_ActivateAthlete = "Q_ActivateAthlete";

        public const string Q_AddActivity = "Q_AddActivity";
        public const string Q_UpdateActivity = "Q_UpdateActivity";
        public const string Q_DeleteActivity = "Q_DeleteActivity";

        public const string T_CalculateMonthlyAthleteResults = "T_CalculateMonthlyAthleteResults";
        public const string T_CalculateMonthlyAthleteResultsFromPreviousMonth = "T_CalculateMonthlyAthleteResultsFromPreviousMonth";
        public const string Q_CalculateMonthlyAthleteResults = "Q_CalculateMonthlyAthleteResults";

        public const string Q_NotificationsGateway = "Q_NotificationsGateway";
        public const string T_GenerateReport = "T_GenerateReport";
        public const string B_SendNotificationWithLinkToTheReport = "B_SendNotificationWithLinkToTheReport";
    }
}