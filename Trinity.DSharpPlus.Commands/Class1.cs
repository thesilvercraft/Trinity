using DSharpPlus.Entities;
using System.Globalization;
using System.Text.RegularExpressions;
using Trinity.Commands;
using Trinity.Commands.Converters;
using Trinity.DiscordSharpPlus;
using Emzi0767;
using Optional = Emzi0767.Optional;

namespace Trinity.DSharpPlus.Commands
{
    public static class TrinityDiscordCommandsNextConvertersExtension
    {
        public static void RegisterDiscordConverters(this CommandsNextExtension extension)
        {
            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordUserConverter());
            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordMemberConverter());
            extension.RegisterUserFriendlyTypeName<TrinityDiscordUser>(typeof(TrinityDiscordClient), "Discord user");
            extension.RegisterUserFriendlyTypeName<TrinityDiscordMember>(typeof(TrinityDiscordClient), "Discord member");
        }
    }

    public class DiscordUserConverter : IArgumentConverter<TrinityDiscordUser>
    {
        private static Regex UserRegex { get; }

        static DiscordUserConverter()
        {
            UserRegex = new Regex(@"^<@\!?(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        public async Task<Emzi0767.Optional<TrinityDiscordUser>> ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Client is TrinityDiscordClient client)
            {
                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    var result = await client.client.GetUserAsync(uid).ConfigureAwait(false);
                    var ret = result != null ? Optional.FromValue(new TrinityDiscordUser(result)) : Optional.FromNoValue<TrinityDiscordUser>();
                    return ret;
                }

                var m = UserRegex.Match(value);
                if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uid))
                {
                    var result = await client.client.GetUserAsync(uid).ConfigureAwait(false);
                    var ret = result != null ? Optional.FromValue(new TrinityDiscordUser(result)) : Optional.FromNoValue<TrinityDiscordUser>();
                    return ret;
                }

                var cs = ctx.Config.CaseSensitive;

                var di = value.IndexOf('#');
                var un = di != -1 ? value.Substring(0, di) : value;
                var dv = di != -1 ? value.Substring(di + 1) : null;

                var us = client.client.Guilds.Values
                    .SelectMany(xkvp => xkvp.Members.Values).Where(xm =>
                        xm.Username.Equals(un, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) &&
                        ((dv != null && xm.Discriminator == dv) || dv == null));

                var usr = us.FirstOrDefault();
                return usr != null ? Optional.FromValue<TrinityDiscordUser>(new(usr)) : Optional.FromNoValue<TrinityDiscordUser>();
            }
            else
            {
                return Optional.FromNoValue<TrinityDiscordUser>();
            }
        }
    }

    public class DiscordMemberConverter : IArgumentConverter<TrinityDiscordMember>
    {
        private static Regex UserRegex { get; }

        static DiscordMemberConverter()
        {
            UserRegex = new Regex(@"^<@\!?(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        async Task<Emzi0767.Optional<TrinityDiscordMember>> IArgumentConverter<TrinityDiscordMember>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Guild == null)
                return Optional.FromNoValue<TrinityDiscordMember>();
            if (ctx.Client is TrinityDiscordClient client && ctx.Guild is TrinityDiscordGuild guild)
            {
                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    var result = await guild.Guild.GetMemberAsync(uid).ConfigureAwait(false);
                    var ret = result != null ? Emzi0767.Optional.FromValue(new TrinityDiscordMember(result)) : Emzi0767.Optional.FromNoValue<TrinityDiscordMember>();
                    return ret;
                }

                var m = UserRegex.Match(value);
                if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uid))
                {
                    var result = await guild.Guild.GetMemberAsync(uid).ConfigureAwait(false);
                    return result != null ? Optional.FromValue(new TrinityDiscordMember(result)) : Optional.FromNoValue<TrinityDiscordMember>();
                }

                var searchResult = await guild.Guild.SearchMembersAsync(value).ConfigureAwait(false);
                if (searchResult.Any())
                    return Optional.FromValue(new TrinityDiscordMember(searchResult.First()));

                var cs = ctx.Config.CaseSensitive;

                var di = value.IndexOf('#');
                var un = di != -1 ? value.Substring(0, di) : value;
                var dv = di != -1 ? value.Substring(di + 1) : null;

                var comparison = cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
                var us = guild.Guild.Members.Values.Where(xm =>
                    (xm.Username.Equals(un, comparison) &&
                     ((dv != null && xm.Discriminator == dv) || dv == null)) || value.Equals(xm.Nickname, comparison));

                var mbr = us.FirstOrDefault();
                return mbr != null ? Optional.FromValue(new TrinityDiscordMember(mbr)) : Optional.FromNoValue<TrinityDiscordMember>();
            }
            else
            {
                return Optional.FromNoValue<TrinityDiscordMember>();
            }
        }
    }

    //TODO
    /*
    public class DiscordChannelConverter : IArgumentConverter<DiscordChannel>
    {
        private static Regex ChannelRegex { get; }

        static DiscordChannelConverter()
        {
#if NETSTANDARD1_3
        ChannelRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript);
#else
            ChannelRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
#endif
        }

        async Task<Optional<DiscordChannel>> IArgumentConverter<DiscordChannel>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cid))
            {
                var result = await ctx.Client.GetChannelAsync(cid).ConfigureAwait(false);
                return result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordChannel>();
            }

            var m = ChannelRegex.Match(value);
            if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cid))
            {
                var result = await ctx.Client.GetChannelAsync(cid).ConfigureAwait(false);
                return result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordChannel>();
            }

            var cs = ctx.Config.CaseSensitive;

            var comparison = cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            var chn = ctx.Guild?.Channels.Values.FirstOrDefault(xc => xc.Name.Equals(value, comparison)) ??
                      ctx.Guild?.Threads.Values.FirstOrDefault(xThread => xThread.Name.Equals(value, comparison));

            return chn != null ? Optional.FromValue(chn) : Optional.FromNoValue<DiscordChannel>();
        }
    }

    public class DiscordThreadChannelConverter : IArgumentConverter<DiscordThreadChannel>
    {
        private static Regex ThreadRegex { get; }

        static DiscordThreadChannelConverter()
        {
#if NETSTANDARD1_3
        ThreadRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript);
#else
            ThreadRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
#endif
        }

        Task<Optional<DiscordThreadChannel>> IArgumentConverter<DiscordThreadChannel>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var threadId))
            {
                var result = ctx.Client.InternalGetCachedThread(threadId);
                return Task.FromResult(result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordThreadChannel>());
            }

            var m = ThreadRegex.Match(value);
            if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out threadId))
            {
                var result = ctx.Client.InternalGetCachedThread(threadId);
                return Task.FromResult(result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordThreadChannel>());
            }

            var cs = ctx.Config.CaseSensitive;

            var thread = ctx.Guild?.Threads.Values.FirstOrDefault(xt =>
                xt.Name.Equals(value, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase));

            return Task.FromResult(thread != null ? Optional.FromValue(thread) : Optional.FromNoValue<DiscordThreadChannel>());
        }
    }

    public class DiscordRoleConverter : IArgumentConverter<DiscordRole>
    {
        private static Regex RoleRegex { get; }

        static DiscordRoleConverter()
        {
            RoleRegex = new Regex(@"^<@&(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        Task<Optional<DiscordRole>> IArgumentConverter<DiscordRole>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Guild == null)
                return Task.FromResult(Optional.FromNoValue<DiscordRole>());

            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rid))
            {
                var result = ctx.Guild.GetRole(rid);
                var ret = result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordRole>();
                return Task.FromResult(ret);
            }

            var m = RoleRegex.Match(value);
            if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out rid))
            {
                var result = ctx.Guild.GetRole(rid);
                var ret = result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordRole>();
                return Task.FromResult(ret);
            }

            var cs = ctx.Config.CaseSensitive;

            var rol = ctx.Guild.Roles.Values.FirstOrDefault(xr =>
                xr.Name.Equals(value, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(rol != null ? Optional.FromValue(rol) : Optional.FromNoValue<DiscordRole>());
        }
    }

    public class DiscordGuildConverter : IArgumentConverter<DiscordGuild>
    {
        Task<Optional<DiscordGuild>> IArgumentConverter<DiscordGuild>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gid))
            {
                return ctx.Client.Guilds.TryGetValue(gid, out var result)
                    ? Task.FromResult(Optional.FromValue(result))
                    : Task.FromResult(Optional.FromNoValue<DiscordGuild>());
            }

            var cs = ctx.Config.CaseSensitive;

            var gld = ctx.Client.Guilds.Values.FirstOrDefault(xg =>
                xg.Name.Equals(value, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(gld != null ? Optional.FromValue(gld) : Optional.FromNoValue<DiscordGuild>());
        }
    }

    public class DiscordMessageConverter : IArgumentConverter<DiscordMessage>
    {
        private static Regex MessagePathRegex { get; }

        static DiscordMessageConverter()
        {
            MessagePathRegex = new Regex(@"^\/channels\/(?<guild>(?:\d+|@me))\/(?<channel>\d+)\/(?<message>\d+)\/?$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        async Task<Optional<DiscordMessage>> IArgumentConverter<DiscordMessage>.ConvertAsync(string value, CommandContext ctx)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Optional.FromNoValue<DiscordMessage>();

            var msguri = value.StartsWith("<") && value.EndsWith(">") ? value.Substring(1, value.Length - 2) : value;
            ulong mid;
            if (Uri.TryCreate(msguri, UriKind.Absolute, out var uri))
            {
                if (uri.Host != "discordapp.com" && uri.Host != "discord.com" && !uri.Host.EndsWith(".discordapp.com") && !uri.Host.EndsWith(".discord.com"))
                    return Optional.FromNoValue<DiscordMessage>();

                var uripath = MessagePathRegex.Match(uri.AbsolutePath);
                if (!uripath.Success
                    || !ulong.TryParse(uripath.Groups["channel"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cid)
                    || !ulong.TryParse(uripath.Groups["message"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
                    return Optional.FromNoValue<DiscordMessage>();

                var chn = await ctx.Client.GetChannelAsync(cid).ConfigureAwait(false);
                if (chn == null)
                    return Optional.FromNoValue<DiscordMessage>();

                var msg = await chn.GetMessageAsync(mid).ConfigureAwait(false);
                return msg != null ? Optional.FromValue(msg) : Optional.FromNoValue<DiscordMessage>();
            }

            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
            {
                var result = await ctx.Channel.GetMessageAsync(mid).ConfigureAwait(false);
                return result != null ? Optional.FromValue(result) : Optional.FromNoValue<DiscordMessage>();
            }

            return Optional.FromNoValue<DiscordMessage>();
        }
    }
}*/
}