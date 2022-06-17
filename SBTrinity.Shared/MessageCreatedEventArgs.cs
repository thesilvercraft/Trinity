using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public class MessageCreatedEventArgs : TrinityEventArgs
    {
        public MessageCreatedEventArgs(ITrinityMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message that was created.
        /// </summary>
        public ITrinityMessage Message { get; internal set; }

        /// <summary>
        /// Gets the channel this message belongs to.
        /// </summary>
        public ITrinityChannel Channel
            => Message.Channel;

        /// <summary>
        /// Gets the guild this message belongs to.
        /// </summary>
        public ITrinityGuild? Guild
            => Channel.Guild;

        /// <summary>
        /// Gets the author of the message.
        /// </summary>
        public ITrinityUser Author
            => Message.Author;

        public IReadOnlyList<Mention> Mentions => Message.Mentions;
    }
}