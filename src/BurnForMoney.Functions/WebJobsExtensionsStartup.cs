using System.Linq;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

[assembly: WebJobsStartup(typeof(BurnForMoney.Functions.WebJobsExtensionStartup))]
namespace BurnForMoney.Functions
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var serviceConfig = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IConfiguration));
            var rootConfig = (IConfiguration)serviceConfig.ImplementationInstance;

            var keyvaultName = rootConfig["KeyVaultName"];
            var config = new ConfigurationBuilder()
                .AddConfiguration(rootConfig).AddAzureKeyVault($"https://{keyvaultName}.vault.azure.net/").Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.AddExtension(new ConfigurationExtensionConfigProvider<Configuration.ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            InitEventSource(config);
        }

        private static void InitEventSource(IConfigurationRoot config)
        {
            var storageAccount = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);
            var tableClient = storageAccount.CreateCloudTableClient();
            var domainEventsTable = tableClient.GetTableReference("DomainEvents");
            domainEventsTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }
    }
}