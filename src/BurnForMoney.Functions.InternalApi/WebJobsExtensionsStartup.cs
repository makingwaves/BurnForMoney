using System;
using System.Linq;
using BurnForMoney.Functions.InternalApi;
using BurnForMoney.Functions.InternalApi.Configuration;
using BurnForMoney.Functions.Shared.Functions.Extensions;
using BurnForMoney.Infrastructure.Authorization.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            builder.AddExtension(new ConfigurationExtensionConfigProvider<Configuration.ConfigurationRoot>(
                ApplicationConfiguration.GetSettings()));

            if(rootConfig.GetValue("UseLocalAuthentication", false))
                builder.AddBfmAuthorization(new DevelopBfmPrincipalFactory());
            else
                builder.AddBfmAuthorization(new BfmPrincipalFactory());
        }
    }
}