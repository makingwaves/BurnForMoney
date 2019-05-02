using System.Data.SqlClient;
using System.Linq;
using BurnForMoney.Functions.PublicApi.Calculators;
using BurnForMoney.Functions.PublicApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ConfigurationRoot = BurnForMoney.Functions.PublicApi.Configuration.ConfigurationRoot;

[assembly: WebJobsStartup(typeof(BurnForMoney.Functions.PublicApi.WebJobsExtensionStartup))]
namespace BurnForMoney.Functions.PublicApi
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var serviceConfig = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IConfiguration));
            // ReSharper disable once PossibleNullReferenceException
            var rootConfig = (IConfiguration)serviceConfig.ImplementationInstance;

            var keyvaultName = rootConfig["KeyVaultName"];
            var config = new ConfigurationBuilder()
                .AddConfiguration(rootConfig).AddAzureKeyVault($"https://{keyvaultName}.vault.azure.net/").Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.AddExtension(new ConfigurationExtensionConfigProvider<ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            builder.AddDependencyInjection(ConfigureServices);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ApplicationConfiguration.GetSettings());
            services.AddScoped<IConnectionProvider<SqlConnection>>(factory =>
            {
                var config = factory.GetService<ConfigurationRoot>();
                return new SqlConnectionProvider(config.ConnectionStrings.SqlDbConnectionString);
            });
            services.AddScoped<IEmployeesEngagementCalculator>(factory =>
            {
                var config = factory.GetService<ConfigurationRoot>();
                return new EmployeesEngagementCalculator(config.CompanyInformation.NumberOfEmployees);
            });
            services.AddScoped<ITotalNumbersCalculator, TotalNumbersCalculator>();
        }
    }
}