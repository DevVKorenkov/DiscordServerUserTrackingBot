using DsBot.ActiveDirectorySync;
using DsBot.ActiveDirectorySync.Infrastructure;
using DsBot.ActiveDirectorySync.Services.Abstractions;
using DsBot.ActiveDirectorySync.Services;
using DsBot.ActiveDirectorySync.Infrastructure.Abstractions;
using DsBot.ActiveDirectorySync.QueryServer.Api;
using MongoDB.Driver;
using DsBot.ConfigureSyncUserLib.Settings;
using DsBot.ConfigureSyncUserLib.Repositories.Abstrations;
using DsBot.ConfigureSyncUserLib.Repositories;
using DsBot.ConfigureSyncUserLib.MessageSenders;
using Microsoft.AspNetCore.Builder;
using DsBot.ConfigureSyncUserLib.Configure;

const string mongoConnectionString = "MongoConnectionString";

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
var rabbitMqSettings = services.LoadSettingsSection<RabbitMqSettings>(configuration);
services.ConfigureRabbitMq(rabbitMqSettings);
services.AddHostedService<Worker>();
services.AddTransient<IMessageSender, RabbitMqMessageSender>();
services.AddTransient<IUserService, UserService>();

services.SetSettingsFromSection<ServiceSettings>(configuration);
services.SetSettingsFromSection<UserDbSettings>(configuration);
services.SetSettingsFromSection<EventDbSettings>(configuration);

var connectionString = configuration.GetConnectionString(mongoConnectionString);
services.AddSingleton<IMongoClient>(m => new MongoClient(connectionString));
services.AddSingleton<IActiveDirectoryRepository, ActiveDirectoryRepository>();
services.AddSingleton<IUserRepository, UserRepository>();
services.AddSingleton<IEventRepository, EventRepository>();

services.AddGraphQLServer().AddQueryType<Query>();
services.AddControllers();
        
var app = builder.Build();

app.MapControllers();
app.MapGraphQL();
app.Run();