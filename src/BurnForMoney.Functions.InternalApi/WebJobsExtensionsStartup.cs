using System.Data.SqlClient;
using System.Linq;
using BurnForMoney.Functions.InternalApi;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ConfigurationRoot = BurnForMoney.Functions.InternalApi.Configuration.ConfigurationRoot;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup))]
namespace BurnForMoney.Functions.InternalApi
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
            services.AddScoped<IAccountsStore>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new AccountsStore(configuration.ConnectionStrings.AzureAccountsStorage);
            });
            services.AddScoped<IAthleteReadRepository, AthleteReadRepository>();
        }
    }
}