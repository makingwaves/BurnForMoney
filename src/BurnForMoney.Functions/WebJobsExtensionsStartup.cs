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
using Microsoft.AspNetCore.Hosting;
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
            
            builder.AddExtension(new ConfigurationExtensionConfigProvider<ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            InitEventSource(config);
            builder.AddDependencyInjection(x => ConfigureServices(x, ApplicationConfiguration.GetSettings()));       
        }

        private static void InitEventSource(IConfigurationRoot config)
        {
            var storageAccount = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);
            var tableClient = storageAccount.CreateCloudTableClient();
            var domainEventsTable = tableClient.GetTableReference("DomainEvents");
            domainEventsTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var accountsTable = tableClient.GetTableReference("Accounts");
            accountsTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        private void ConfigureServices(IServiceCollection services, ConfigurationRoot root)
        {
            services.AddSingleton(root);
            services.AddScoped<IConnectionFactory<SqlConnection>, SqlConnectionFactory>();
            services.AddScoped<IRepository<Athlete>>(factory =>
            {
                var store = factory.GetService<IEventStore>();
                return new Repository<Athlete>(store);
            });
            services.AddScoped<IEventPublisher>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint);
            });
            services.AddScoped(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                var publisher = factory.GetService<IEventPublisher>();
                return EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage, publisher);
            });
            services.AddScoped<IAccountsStore>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new AccountsStore(configuration.ConnectionStrings.AzureWebJobsStorage);
            });
            services.AddScoped<IAthleteReadRepository>(factory =>
            {
                var configuration = factory.GetService<ConfigurationRoot>();
                return new AthleteReadRepository(configuration.ConnectionStrings.SqlDbConnectionString);
            });
            services.AddScoped<ICommandHandler<ActivateAthleteCommand>, ActivateAthleteCommandHandler>();
            services.AddScoped<ICommandHandler<AddActivityCommand>, AddActivityCommandHandler>();
            services.AddScoped<ICommandHandler<AddStravaAccountCommand>, AddStravaAccountCommandHandler>();
            services.AddScoped<ICommandHandler<CreateAthleteCommand>, CreateAthleteCommandHandler>();
            services.AddScoped<ICommandHandler<DeactivateAthleteCommand>, DeactivateAthleteCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteActivityCommand>, DeleteActivityCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateActivityCommand>, UpdateActivityCommandHandler>();
        }
    }
}