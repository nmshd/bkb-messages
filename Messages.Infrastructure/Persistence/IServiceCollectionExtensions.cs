using Messages.Infrastructure.Persistence.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.Infrastructure.Persistence;

public static class IServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<PersistenceOptions> setupOptions)
    {
        var options = new PersistenceOptions();
        setupOptions?.Invoke(options);

        services.AddPersistence(options);
    }

    public static void AddPersistence(this IServiceCollection services, PersistenceOptions options)
    {
        services.AddDatabase(options.DbOptions);

        services.AddAzureStorageAccount(options.BlobStorageOptions);
    }
}

public class PersistenceOptions
{
    public DbOptions DbOptions { get; set; } = new();
    public AzureStorageAccountOptions BlobStorageOptions { get; set; } = new();
}
