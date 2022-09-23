using DiscordServerUserTrackingBot.DiscordBotClientService.MessageSender.Abstractions;
using DiscordServerUserTrackingBot.DiscordBotClientService.Settings;
using MassTransit;
using Microsoft.Extensions.Options;

namespace DiscordServerUserTrackingBot.DiscordBotClientService.MessageSender
{
    public class RabbitMqMessageSender : IMessageSender
    {
        private readonly ILogger<RabbitMqMessageSender> _logger;
        private readonly IBus _bus;
        private readonly RabbitMqSettings _settings;
        public RabbitMqMessageSender(ILogger<RabbitMqMessageSender> logger, IBus bus, IOptions<RabbitMqSettings> settings)
        {
            _logger = logger;
            _bus = bus;
            _settings = settings.Value;
        }

        public async Task<bool> TrySendAsync<T>(T message, CancellationToken stoppingToken)
        {
            try
            {
                var cancellationTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cancellationTokenSource.CancelAfter(
                    TimeSpan.FromMilliseconds(_settings.TimeoutMs));
                await _bus.Publish(message, cancellationTokenSource.Token);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogInformation("RabbitMqMessageSender exception: " + e);
                return false;
            }
        }
    }
}
