namespace BurnForMoney.Functions.PublicApi.Calculators
{
    public interface IEmployeesEngagementCalculator
    {
        int GetPercentOfEngagedEmployees(int numberOfTheUniqueAthletes);
    }

    public class EmployeesEngagementCalculator : IEmployeesEngagementCalculator
    {
        private readonly int _numberOfEmployees;

        public EmployeesEngagementCalculator(int numberOfEmployees)
        {
            _numberOfEmployees = numberOfEmployees;
        }

        public int GetPercentOfEngagedEmployees(int numberOfTheUniqueAthletes) =>
            (numberOfTheUniqueAthletes * 100) / _numberOfEmployees;
    }
}