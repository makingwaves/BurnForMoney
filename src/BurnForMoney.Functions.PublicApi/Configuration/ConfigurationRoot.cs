namespace BurnForMoney.Functions.PublicApi.Configuration
{
    public class ConfigurationRoot
    {
        public ConnectionStringsSection ConnectionStrings { get; set; }

        public CompanyInformationSection CompanyInformation { get; set; }

        public bool IsValid()
        {
            return ConnectionStrings != null;
        }
    }

    public class ConnectionStringsSection
    {
        public string SqlDbConnectionString { get; set; }
    }

    public class CompanyInformationSection
    {
        private const int DefaultNumberOfEmployees = 100;

        private int _numberOfEmployees;

        public int NumberOfEmployees
        {
            get => _numberOfEmployees > 0 ? _numberOfEmployees : DefaultNumberOfEmployees;

            set
            {
                if (value > 0)
                {
                    _numberOfEmployees = value;
                }
            }
        }
    }
}