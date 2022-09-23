namespace DiscordServerUserTrackingBot.DiscordBotClientService.Settings
{
    public class DiscordBotSettings
    {
        public string BotToken { get; set; }
        public string WelcomeMessage { get; set; }
        public string ReRegistrationMessage { get; set; }
        public string AuthenticationUrl {get; set;}
        public ulong[] Roles{ get; set;}
        public string RolesGrantedMessage { get; set; }
        public double ReadinessWaitTimeoutMs { get; set; }
        public int BotDisconnectedDelay { get; set; }
        public int ConnectionAttemptsCount { get; set; }
    }
}
