using Microsoft.AspNetCore.Mvc;
using DiscordServerUserTrackingBot.ConfigureSyncUserLib.Models;
using DiscordServerUserTrackingBot.YandexSyncService.Services.Abstractions;

namespace DiscordServerUserTrackingBot.YandexSyncService.Controllers
{
    [Route("discord")]
    public class DiscordController : Controller
    {
        private readonly IUserService _userService;

        public DiscordController(
            IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("users")]
        public async Task ProcessingUsers([FromBody] UserCreds userBody)
        {
            await _userService.UserProcessing(userBody);
        }
    }
}
