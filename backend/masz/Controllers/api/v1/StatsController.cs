
using System.Collections.Generic;
using System.Threading.Tasks;
using masz.Dtos.DiscordAPIResponses;
using masz.Models;
using masz.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1/stats/")]
    public class StatsController : ControllerBase
    {
        private readonly ILogger<StatsController> logger;
        private readonly IDatabase database;
        private readonly IOptions<InternalConfig> config;
        private readonly IIdentityManager identityManager;
        private readonly IDiscordAPIInterface discord;

        public StatsController(ILogger<StatsController> logger, IDatabase database, IOptions<InternalConfig> config, IIdentityManager identityManager, IDiscordAPIInterface discordInterface)
        {
            this.logger = logger;
            this.database = database;
            this.config = config;
            this.identityManager = identityManager;
            this.discord = discordInterface;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGuildStats([FromRoute] string guildid, [FromRoute] string modcaseid) 
        {
            List<GuildConfig> guildConfigs = await database.SelectAllGuildConfigs();
            List<ModCase> modCases = await database.SelectAllModCases();            
            return Ok(new {
                guilds = guildConfigs.Count,
                modCaseCount = modCases.Count,
                autoModerations = await database.CountAllModerationEvents()
            });
        }

        [HttpGet("{guildid}")]
        public async Task<IActionResult> GetGuildStats([FromRoute] string guildid) 
        {
            if (await database.SelectSpecificGuildConfig(guildid) == null)
            {
                logger.LogInformation($"{HttpContext.Request.Method} {HttpContext.Request.Path} | 400 Guild not registered.");
                return BadRequest("Guild not registered.");
            }

            List<ModCase> modCases = await database.SelectAllModCasesForGuild(guildid);
            List<ModCase> activePunishments = await database.SelectAllModCasesWithActivePunishmentForGuild(guildid);
            return Ok(new {
                modCaseCount = modCases.Count,
                activePunishments = activePunishments.Count,
                autoModerations = await database.CountAllModerationEventsForGuild(guildid)
            });
        }
    }
}