﻿using DSharpPlus;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordClient : IPlatformProvider
    {
        public TrinityDiscordClient(DiscordClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            client.MessageCreated += Client_MessageCreated;
        }

        private async Task Client_MessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (MessageRecieved != null)
            {
                await MessageRecieved.Invoke(this, new MessageRecievedArgs(new TrinityDiscordMessage(e.Message)));
            }
        }

        public DiscordClient client { get; set; }

        public event AsyncEvent<IPlatformProvider, MessageRecievedArgs> MessageRecieved;

        public Task ConnectAsync() => client.ConnectAsync();

        public Task DisconnectAsync() => client.DisconnectAsync();

        public Task ReConnectAsync() => client.ReconnectAsync();

        public async Task<ITrinityChannel?> GetChannelAsync(TrinityGuid channelId)
        {
            if (channelId is TrinityUlongGuid id)
            {
                var value = await client.GetChannelAsync(id.Value);
                return value == null ? null : new TrinityDiscordChannel(value);
            }
            throw new NotSupportedException($"The current implementation of {nameof(TrinityDiscordClient)}.{nameof(GetChannelAsync)} does not support accepting a {channelId.GetType()} as an input paramater try using a TrinityUlongGuid instead");
        }

        public async Task<ITrinityGuild?> GetGuildAsync(TrinityGuid guildId)
        {
            if (guildId is TrinityUlongGuid id)
            {
                var value = await client.GetGuildAsync(id.Value);
                return value == null ? null : new TrinityDiscordGuild(value);
            }
            throw new NotSupportedException($"The current implementation of {nameof(TrinityDiscordClient)}.{nameof(GetGuildAsync)} does not support accepting a {guildId.GetType()} as an input paramater try using a TrinityUlongGuid instead");
        }

        public List<ITrinityGuild> GetCachedGuilds()
        {
            return client.Guilds.Values.Select(x => (ITrinityGuild)new TrinityDiscordGuild(x)).ToList();
        }
    }
}