using DSharpPlus.Entities;
using Trinity.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordEmbed : TrinityEmbed
    {
        private DiscordEmbed x;

        public TrinityDiscordEmbed(DiscordEmbed x)
        {
            this.x = x;
        }

        public static List<TrinityDiscordEmbed> CreateFromDiscordEmbedList(IReadOnlyList<DiscordEmbed> list)
        {
            return list.Select(x => new TrinityDiscordEmbed(x)).ToList();
        }

        public static List<TrinityEmbed>? CreateFromDiscordEmbedListAsTrinityEmbed(IReadOnlyList<DiscordEmbed> embeds)
        {
            return embeds.Select(x => (TrinityEmbed)new TrinityDiscordEmbed(x)).ToList();
        }
    }
}