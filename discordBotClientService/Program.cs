using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Configure;
using DiscordServerUserTrackingBot.DiscordBotClientService.Configure;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord.Abstractions;
using DiscordServerUserTrackingBot.DiscordBotClientService.MessageSender;
using DiscordServerUserTrackingBot.DiscordBotClientService.MessageSender.Abstractions;
using DiscordServerUserTrackingBot.DiscordBotClientService.QueryServer.Api;
using DiscordServerUserTrackingBot.DiscordBotClientService.Settings;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddGrpc();
var rabbitMqSettings = services.LoadSettingsSection<RabbitMqSettings>(configuration);
services.ConfigureRabbitMq(rabbitMqSettings);
services.SetSettingsFromSection<DiscordBotSettings>(configuration);
services.AddTransient<IMessageSender, RabbitMqMessageSender>();

services.AddSingleton<BotClientManager>();

services.AddHostedService(services => services.GetService<BotClientManager>());
services.AddSingleton<IBotClient>(services => services.GetService<BotClientManager>());

services.AddGraphQLServer().AddQueryType<Query>();

services.AddControllers();

var app = builder.Build();

app.MapGraphQL();

app.MapControllers();

app.Run();
