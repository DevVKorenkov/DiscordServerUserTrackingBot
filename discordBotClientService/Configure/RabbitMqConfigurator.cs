using DiscordServerUserTrackingBot.DiscordBotClientService.Consumers;
using DiscordServerUserTrackingBot.DiscordBotClientService.Discord;
using DiscordServerUserTrackingBot.DiscordBotClientService.Settings;
using MassTransit;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.Configure;

static class RabbitMqConfigurator
{
    static public void ConfigureRabbitMq(this IServiceCollection serviceCollection, RabbitMqSettings settings)
    {
        serviceCollection.AddMassTransit(x =>
        {
            x.AddConsumer<DiscordUserWasArchivedConsumer>();
            x.AddConsumer<AddRolesToNewDiscordUserConsumer>();
            x.AddConsumer<DeleteDiscordUserConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseMessageRetry(
                    r =>
                    {
                        r.Interval(settings.RetryCount, TimeSpan.FromMilliseconds(settings.RetryIntervalMs));
                        r.Ignore<WrongDiscordUserIdStringException>();
                        r.Ignore<UserNotFoundException>();
                    });
                cfg.ConfigureEndpoints(context);

                cfg.Host(new Uri(settings.Url), h =>
                {
                    h.Username(settings.User);
                    h.Password(settings.Password);
                });
            });
        });

        serviceCollection.AddOptions<MassTransitHostOptions>()
            .Configure(options => { options.WaitUntilStarted = true; });
    }
}
