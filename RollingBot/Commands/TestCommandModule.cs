using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Commands;
using Trinity.Commands.Attributes;
using Trinity.DiscordSharpPlus;
using Trinity.Shared;

namespace RollingBot.Commands
{
    internal class TestCommandModule : BaseCommandModule
    {
        [Command("ay")]
        public async Task Gaming(CommandContext ctx, ITrinityUser arg)
        {
            await ctx.RespondAsync($"{arg.Id.ToGuid()}");
        }
    }
}