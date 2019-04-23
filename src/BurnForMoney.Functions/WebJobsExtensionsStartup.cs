using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BurnForMoney.Functions.CommandHandlers;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Persistence;
using BurnForMoney.Infrastructure.Persistence.Repositories;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ConfigurationRoot = BurnForMoney.Functions.Configuration.ConfigurationRoot;

[assembly: WebJobsStartup(typeof(BurnForMoney.Functions.WebJobsExtensionStartup))]
namespace BurnForMoney.Functions
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
            builder.Services.AddSingleton(factory => ApplicationConfiguration.GetSettings());
            builder.AddExtension(new ConfigurationExtensionConfigProvider<ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            InitEventSource(config);

            builder.AddDependencyInjection(ConfigureServices);
        }

        private static void InitEventSource(IConfigurationRoot config)
        {
            var storageAccount = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);
            var tableClient = storageAccount.CreateCloudTableClient();
            var domainEventsTable = tableClient.GetTableReference("DomainEvents");
            domainEventsTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConnectionFactory<SqlConnection>, SqlConnectionFactory>();
            services.AddTransient<IRepository<Athlete>, Repository<Athlete>>();
            services.AddTransient<IEventPublisher>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint);
            });
            services.AddTransient(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                var publisher = factory.GetService<IEventPublisher>();
                return EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage, publisher);
            });
            services.AddTransient<IAthleteReadRepository>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            });
            services.AddTransient<ICommandHandler<ActivateAthleteCommand>, ActivateAthleteCommandHandler>();
            services.AddTransient<ICommandHandler<AddActivityCommand>, AddActivityCommandHandler>();
            services.AddTransient<ICommandHandler<AddStravaAccountCommand>, AddStravaAccountCommandHandler>();
            services.AddTransient<ICommandHandler<AssignActiveDirectoryIdToAthleteCommand>, AssignActiveDirectoryIdToAthleteCommandHandler>();
            services.AddTransient<ICommandHandler<CreateAthleteCommand>, CreateAthleteCommandHandler>();
            services.AddTransient<ICommandHandler<DeactivateAthleteCommand>, DeactivateAthleteCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteActivityCommand>, DeleteActivityCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateActivityCommand>, UpdateActivityCommandHandler>();
        }
    }
}