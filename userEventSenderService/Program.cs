using MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using DiscordServerUserTrackingBot.UserEventSenderService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Configure;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.MessageSenders;
using DiscordServerUserTrackingBot.UserEventSenderService.Services;
using DiscordServerUserTrackingBot.UserEventSenderService.Services.Abstractions;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories;

const string mongoConnectionString = "MongoConnectionString";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var rabbitMqSettings = services.LoadSettingsSection<RabbitMqSettings>(hostContext);
        services.ConfigureRabbitMq(rabbitMqSettings);
        services.AddHostedService<Worker>();
        services.AddTransient<IMessageSender, RabbitMqMessageSender>();
        services.AddTransient<IEventService, EventService>();

        services.SetSettingsFromSection<EventDelaySettings>(hostContext);
        services.SetSettingsFromSection<EventDbSettings>(hostContext);

        var connectionString = hostContext.Configuration.GetConnectionString(mongoConnectionString);
        services.AddSingleton<IMongoClient>(m => new MongoClient(connectionString));
        services.AddSingleton<IEventRepository, EventRepository>();
    })
    .Build();

await host.RunAsync();
