//using DiscordServerUserTrackingBot.SilentUsersRemovingService.Consumers;
using DiscordServerUserTrackingBot.SilentUsersRemovingService.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordServerUserTrackingBot.SilentUsersRemovingService.Configure;

static class RabbitMqConfigurator
{
    public static void ConfigureRabbitMq(this IServiceCollection serviceCollection, HostBuilderContext hostBuilderContext)
    {
        var settings = serviceCollection.LoadSettingsSection<RabbitMqSettings>(hostBuilderContext);
        serviceCollection.AddMassTransit(x =>
        {
            //x.AddConsumer<AuthorizeRequestSentConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(settings.Url), h =>
                {
                    h.Username(settings.User);
                    h.Password(settings.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
