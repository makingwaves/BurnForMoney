using AspNetCore.Proxy;
using BurnForMoney.ApiGateway.Authentication;
using BurnForMoney.ApiGateway.Clients;
using BurnForMoney.ApiGateway.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Blob;
using Swashbuckle.AspNetCore.Swagger;

namespace BurnForMoney.ApiGateway
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfiguration>(_configuration.GetSection("AppConfig"));
            services.Configure<OidcConfiguration>(_configuration.GetSection("OpenIdConnect"));

            var dataProtectionConfiguration = new DataProtectionConfiguration();
            _configuration.Bind("DataProtection", dataProtectionConfiguration);

            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    new AzureServiceTokenProvider().KeyVaultTokenCallback
                )
            );

            services
                .AddDataProtection()
                .PersistKeysToAzureBlobStorage(
                    new CloudBlobContainer(dataProtectionConfiguration.KeyPersistenceBlobAddress),
                    dataProtectionConfiguration.KeyPersistenceBlobName
                )
                .ProtectKeysWithAzureKeyVault(keyVaultClient, dataProtectionConfiguration.KeysProtectionKeyVault);

            services
                .AddSingleton<IBfmApiClient, HttpBfmApiClient>();

            services
                .AddAuthentication()
                .AddBfmAuth(_configuration);

            services.AddScoped<BfmOidcServerProvider>();
            services.AddSingleton<IRedirectUriValidator, RedirectUriValidator>();
            services
                .AddProxies()
                .AddMvc();

            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new Info
                {
                    Title = "API Gateway",
                    Version = "v1"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(options =>
                {
                    options.AllowAnyMethod();
                    options.AllowCredentials();
                    options.AllowAnyHeader();
                    options.WithOrigins("http://localhost:3000", "http://localhost");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication()
                .UseMvc()
                .UseHttpsRedirection()
                .UseSwagger()
                .UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
                    setup.RoutePrefix = string.Empty;
                });
        }
    }
}