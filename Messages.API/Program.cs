using Azure.Identity;
using Enmeshed.BuildingBlocks.API.Extensions;
using Enmeshed.Tooling.Extensions;
using Messages.API;
using Messages.API.Extensions;
using Messages.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore;

CreateWebHostBuilder(args)
    .Build()
    .MigrateDbContext<ApplicationDbContext>((_, _) => { })
    .Run();

static IWebHostBuilder CreateWebHostBuilder(string[] args)
{
    return WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
        {
            options.AddServerHeader = false;
            options.Limits.MaxRequestBodySize = 2.Mebibytes();
        })
        .ConfigureAppConfiguration(AddAzureAppConfiguration)
        .UseStartup<Startup>();
}

static void AddAzureAppConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder builder)
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
