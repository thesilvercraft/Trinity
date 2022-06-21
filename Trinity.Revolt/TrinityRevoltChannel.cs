using Revolt;
using Revolt.Channels;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltChannel : ITrinityChannel, ITrinityChannelWithAdvancedSendingMethods
    {
        public TrinityRevoltChannel(string x, RevoltClient client)
        {
            X = x;
            Client = client;
            Channel = Client.Channels.Get(x);
        }

        public TrinityRevoltChannel(Channel c)
        {
            X = c._id;
            Client = c.Client;
            Channel = c;
        }

        public string X { get; }
        public RevoltClient Client { get; }
        public Channel Channel { get; private set; }
        public string? Name { get => throw new NotSupportedException("NOP"); set => throw new NotSupportedException("CANT"); }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(X); }
        public TrinityChannelType Type { get => Channel.ToTrinityChannelType(); set => throw new NotSupportedException("CANT"); }
        public string? Topic { get => throw new NotSupportedException("NOP"); set => throw new NotSupportedException("NOP"); }
        public IList<ITrinityUser> Users { get => Channel.GetMembersAsync().GetAwaiter().GetResult().Select(y => (ITrinityUser)new TrinityRevoltUser(y)).ToList(); }
        public IList<ITrinityMessage> PinnedMessages { get => throw new NotSupportedException("NOP"); }

        public ITrinityGuild? Guild => null;

        public bool IsNSFW { get => false; set => throw new NotImplementedException(); }

        public bool IsPrivate => throw new NotImplementedException();

        public Task<IList<ITrinityMessage>> GetMessages(TrinityGuid? before = null, TrinityGuid? after = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, string content)
        {
            throw new NotImplementedException();
        }

        public async Task<ITrinityMessage> SendMessageAsync(string content)
        {
            return new TrinityRevoltMessage(await Channel.SendMessageAsync(content), Channel);
        }

        public Task TriggerTypingAsync()
        {
            return Channel.BeginTypingAsync();
        }

        private byte[] StreamToByteArray(Stream sourceStream, long ResetPos)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                sourceStream.Position = ResetPos;
                return memoryStream.ToArray();
            }
        }

        public async Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            List<string> fileitems = new();
            SelfMessage message = null;
            if (trinityMessageBuilder.Files.Any())
            {
                foreach (var file in trinityMessageBuilder.Files)
                {
                    fileitems.Add(await Client.UploadFile(file.FileName, StreamToByteArray(file.Stream, file.ResetPositionTo ?? 0)));
                }
                for (int f = 0; f < fileitems.Count; f++)
                {
                    if (f == 0)
                    {
                        message = await Channel.SendMessageAsync(trinityMessageBuilder.Content, fileitems[f]);
                    }
                    else
                    {
                        message = await Channel.SendMessageAsync("^", fileitems[f]);
                    }
                }
            }
            else
            {
                message = await Channel.SendMessageAsync(trinityMessageBuilder.Content);
            }
            return new TrinityRevoltMessage(message, Channel);
        }

        public Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityMessage, TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotImplementedException("NOP");
        }
    }

    public static class RevoltHelpers
    {
        public static TrinityChannelType ToTrinityChannelType(this Channel channel)
        {
            return channel.ChannelType switch

            {
                "TextChannel" => TrinityChannelType.Text,
                "VoiceChannel" => TrinityChannelType.Voice,
                _ => TrinityChannelType.Unknown
            };
        }
    }
}