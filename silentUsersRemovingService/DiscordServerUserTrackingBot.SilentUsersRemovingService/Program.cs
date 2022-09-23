using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.Configure;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.Delayed;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.MessageSenders;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.MessageSenders.Abstractions;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.QuerySenders.Abstractions;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.SilentUsers;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.SilentUsers.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Net.Http.Headers;
using UserDbSettings = DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings.UserDbSettings;

const string mongoConnectionString = "MongoConnectionString";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.ConfigureRabbitMq(hostContext);
        
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IEventRepository, EventRepository>();
        services.SetSettingsFromSection<UserDbSettings>(hostContext);
        services.SetSettingsFromSection<EventDbSettings>(hostContext);

        var connectionString = hostContext.Configuration.GetConnectionString(mongoConnectionString);
        services.AddSingleton<IMongoClient>(m => new MongoClient(connectionString));

        services.SetSettingsFromSection<UsersRequestSettings>(hostContext);

        services.SetSettingsFromSection<DelayedWorkerSettings>(hostContext);
        services.AddHostedService<DelayedWorker>();
        
        services.ConfigureHangfire(hostContext);
        
        services.AddTransient<ISilentUsersRemover, SilentUsersRemover>();
        services.AddTransient<IMessageSender, RabbitMqMessageSender>();
        services.SetSettingsFromSection<GraphQLClientSettings>(hostContext);
        services.AddTransient<IQuerySender, GraphQLQuerySender>();

        var usersRequestSettings = services.LoadSettingsSection<UsersRequestSettings>(hostContext);
        services.AddHttpClient<UsersRequestSettings>("UsersRequest",
            c =>
            {
                c.BaseAddress = new Uri(usersRequestSettings.Url);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            });
    })
    .Build();

await host.RunAsync();
