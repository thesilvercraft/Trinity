using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Commands;
using Trinity.Commands.Attributes;

namespace RollingBot
{
    internal class TestCommandModule : BaseCommandModule
    {
        [Command("ay")]
        public async Task Gaming(CommandContext ctx, string arg)
        {
            await ctx.RespondAsync($"{arg}");
        }
    }
}