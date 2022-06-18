﻿using System;
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

        [Command("s")]
        public async Task Stress(CommandContext ctx, string arg1, bool arg2, sbyte arg3, byte arg4, short arg5, ushort arg6, int arg7, uint arg8, long arg9, ulong arg10)
        {
            await ctx.RespondAsync("GREAT SUCCESS");
        }

        [Command("s2")]
        public async Task Stress2(CommandContext ctx, float arg1, double arg2, decimal arg3, TimeSpan arg4, Uri arg5)
        {
            await ctx.RespondAsync("GREAT SUCCESS2");
        }
    }
}