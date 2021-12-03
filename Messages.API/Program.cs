using System;
using Azure.Identity;
using Enmeshed.BuildingBlocks.API.Extensions;
using Messages.API.Extensions;
using Messages.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Messages.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build()
                .MigrateDbContext<ApplicationDbContext>((context, services) => { });

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2 MB
                })
                .ConfigureAppConfiguration(AddAzureAppConfiguration)
                .UseStartup<Startup>();
        }

        private static void AddAzureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();

            var azureAppConfigurationConfiguration = configuration.GetAzureAppConfigurationConfiguration();

            if (azureAppConfigurationConfiguration.Enabled)
                builder.AddAzureAppConfiguration(appConfigurationOptions =>
                {
                    var credentials = new ManagedIdentityCredential();

                    appConfigurationOptions
                        .Connect(new Uri(azureAppConfigurationConfiguration.Endpoint), credentials)
                        .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                        .Select("*", "")
                        .Select("*", "Messages");
                });
        }
    }
}
