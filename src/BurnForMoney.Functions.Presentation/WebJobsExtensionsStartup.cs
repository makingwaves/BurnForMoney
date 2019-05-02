using System.Data.SqlClient;
using System.Linq;
using BurnForMoney.Functions.Presentation;
using BurnForMoney.Functions.Presentation.Configuration;
using BurnForMoney.Functions.Presentation.Views;
using BurnForMoney.Functions.Presentation.Views.Mappers;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ConfigurationRoot = BurnForMoney.Functions.Presentation.Configuration.ConfigurationRoot;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup))]

namespace BurnForMoney.Functions.Presentation
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var serviceConfig = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IConfiguration));
            // ReSharper disable once PossibleNullReferenceException
            var rootConfig = (IConfiguration) serviceConfig.ImplementationInstance;

            var keyvaultName = rootConfig["KeyVaultName"];
            var config = new ConfigurationBuilder()
                .AddConfiguration(rootConfig).AddAzureKeyVault($"https://{keyvaultName}.vault.azure.net/").Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.AddExtension(new ConfigurationExtensionConfigProvider<ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            builder.AddDependencyInjection(ConfigureServices);

            RegisterPocoMappings();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ApplicationConfiguration.GetSettings());
            services.AddScoped<IPresentationEventsDispatcherFactory>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new PresentationEventsDispatcherFactory(configuration.ConnectionStrings.SqlDbConnectionString);
            });
            services.AddScoped<IConnectionProvider<SqlConnection>>(factory =>
            {
                var config = factory.GetService<ConfigurationRoot>();
                return new SqlConnectionProvider(config.ConnectionStrings.SqlDbConnectionString);
            });

        }

        private void RegisterPocoMappings()
        {
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new[]
            {
                typeof(ActivityMapper).Assembly
            });
        }
    }
}