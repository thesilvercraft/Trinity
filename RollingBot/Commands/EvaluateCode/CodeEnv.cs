using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Commands;
using Trinity.Shared;

namespace RollingBot.Commands.EvaluateCode
{
    public class CodeEnv
    {
        public CodeEnv(CommandContext context)
        {
            Ctx = context;
            User = Ctx.User;
            Guild = Ctx.Guild;
            Client = Ctx.Client;
        }

        public CommandContext Ctx { get; init; }
        public ITrinityUser User { get; init; }
        public ITrinityGuild Guild { get; init; }
        public IPlatformProvider Client { get; init; }
    }
}