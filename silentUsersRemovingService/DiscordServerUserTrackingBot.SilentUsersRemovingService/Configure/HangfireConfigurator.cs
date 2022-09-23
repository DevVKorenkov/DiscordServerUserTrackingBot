using DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Configure;

public static class HangfireConfigurator
{
    public static void ConfigureHangfire(this IServiceCollection services,
        HostBuilderContext hostBuilderContext)
    {
        var settings = services.LoadSettingsSection<HangfireSettings>(hostBuilderContext);
        var mongoUrlBuilder = 
            new MongoUrlBuilder(settings.MongoUrl)
            {
                DatabaseName = settings.MongoDatabase
            };
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
        
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMongoStorage(
                mongoClient, 
                mongoUrlBuilder.DatabaseName, 
                new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo",
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                }));
        
        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = "Hangfire.Mongo server 1";
        });

    }
}
