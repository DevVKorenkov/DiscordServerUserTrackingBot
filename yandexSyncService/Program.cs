using System.Net.Http.Headers;
using MongoDB.Driver;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Settings;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Repositories;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.MessageSenders;
using Microsoft.AspNetCore.Builder;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Configure;
using DiscordServerUserTrackingBot.YandexSyncService;
using DiscordServerUserTrackingBot.YandexSyncService.Infrastructure;
using DiscordServerUserTrackingBot.YandexSyncService.Infrastructure.Abstractions;
using DiscordServerUserTrackingBot.YandexSyncService.Services;
using DiscordServerUserTrackingBot.YandexSyncService.Services.Abstractions;
using DiscordServerUserTrackingBot.YandexSyncService.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

const string mongoConnectionString = "MongoConnectionString";

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddMemoryCache();

var rabbitMqSettings = services.LoadSettingsSection<RabbitMqSettings>(configuration);
services.ConfigureRabbitMq(rabbitMqSettings);
services.AddHostedService<Worker>();
services.AddTransient<IMessageSender, RabbitMqMessageSender>();
services.AddTransient<IUserService, UserService>();

services.SetSettingsFromSection<ServiceSettings>(configuration);
services.SetSettingsFromSection<UserDbSettings>(configuration);
services.SetSettingsFromSection<EventDbSettings>(configuration);
services.SetSettingsFromSection<YandexSettings>(configuration);

var connectionString = configuration.GetConnectionString(mongoConnectionString);
services.AddSingleton<IMongoClient>(m => new MongoClient(connectionString));
services.AddSingleton<IYandexRepository, YandexRepository>();
services.AddSingleton<IUserRepository, UserRepository>();
services.AddSingleton<IEventRepository, EventRepository>();

var yandexSettings = services.LoadSettingsSection<YandexSettings>(configuration);
services.AddHttpClient<YandexRepository>("SyncUsers",
    c =>
    {
        c.BaseAddress = new Uri(yandexSettings.UsersRequestUrl + yandexSettings.UsersPerPage);
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", yandexSettings.Token);
    });

services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();