using DsBot.ActiveDirectorySync.Services.Abstractions;
using DsBot.ConfigureSyncUserLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace DsBot.ActiveDirectorySync.Controllers;

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
        await _userService.CheckUser(userBody, CancellationToken.None);
    }
}