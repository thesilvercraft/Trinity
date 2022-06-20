// This file is a modified version of a file that is part of the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2022 DSharpPlus Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Globalization;
using System.Text.RegularExpressions;
using Trinity.Commands;
using Trinity.Commands.Converters;
using Trinity.DiscordSharpPlus;
using Emzi0767;
using Optional = Emzi0767.Optional;
using Trinity.Shared;

namespace Trinity.DSharpPlus.Commands
{
    public static class TrinityDiscordCommandsNextConvertersExtension
    {
        public static void RegisterDiscordConverters(this CommandsNextExtension extension)
        {
            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordMemberConverter());
            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordUserConverter());
            extension.RegisterConverter(typeof(TrinityDiscordClient), new TrinityDiscordUserConverter());

            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordMessageConverter());
            extension.RegisterConverter(typeof(TrinityDiscordClient), new TrinityDiscordMessageConverter());

            extension.RegisterConverter(typeof(TrinityDiscordClient), new DiscordGuildConverter());
            extension.RegisterConverter(typeof(TrinityDiscordClient), new TrinityDiscordGuildConverter());

            extension.RegisterUserFriendlyTypeName<TrinityDiscordUser>(typeof(TrinityDiscordClient), "Discord user");
            extension.RegisterUserFriendlyTypeName<TrinityDiscordMember>(typeof(TrinityDiscordClient), "Discord member");
            extension.RegisterUserFriendlyTypeName<TrinityDiscordMessage>(typeof(TrinityDiscordClient), "Discord message");
            extension.RegisterUserFriendlyTypeName<TrinityDiscordGuild>(typeof(TrinityDiscordClient), "Discord guild (server)");
        }
    }

    #region DiscordUser

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
                var un = di != -1 ? value[..di] : value;
                var dv = di != -1 ? value[(di + 1)..] : null;

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

    public class TrinityDiscordUserConverter : IArgumentConverter<ITrinityUser>
    {
        private static Regex UserRegex { get; }

        static TrinityDiscordUserConverter()
        {
            UserRegex = new Regex(@"^<@\!?(\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        public async Task<Emzi0767.Optional<ITrinityUser>> ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Client is TrinityDiscordClient client)
            {
                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
                {
                    var result = await client.client.GetUserAsync(uid).ConfigureAwait(false);
                    var ret = result != null ? Optional.FromValue<ITrinityUser>(new TrinityDiscordUser(result)) : Optional.FromNoValue<ITrinityUser>();
                    return ret;
                }

                var m = UserRegex.Match(value);
                if (m.Success && ulong.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uid))
                {
                    var result = await client.client.GetUserAsync(uid).ConfigureAwait(false);
                    var ret = result != null ? Optional.FromValue<ITrinityUser>(new TrinityDiscordUser(result)) : Optional.FromNoValue<ITrinityUser>();
                    return ret;
                }

                var cs = ctx.Config.CaseSensitive;

                var di = value.IndexOf('#');
                var un = di != -1 ? value[..di] : value;
                var dv = di != -1 ? value[(di + 1)..] : null;

                var us = client.client.Guilds.Values
                    .SelectMany(xkvp => xkvp.Members.Values).Where(xm =>
                        xm.Username.Equals(un, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) &&
                        ((dv != null && xm.Discriminator == dv) || dv == null));

                var usr = us.FirstOrDefault();
                return usr != null ? Optional.FromValue<ITrinityUser>(new TrinityDiscordUser(usr)) : Optional.FromNoValue<ITrinityUser>();
            }
            else
            {
                return Optional.FromNoValue<ITrinityUser>();
            }
        }
    }

    #endregion DiscordUser

    #region DiscordMember

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

    #endregion DiscordMember

    #region DiscordMessage

    public class TrinityDiscordMessageConverter : IArgumentConverter<ITrinityMessage>
    {
        private static Regex MessagePathRegex { get; }

        static TrinityDiscordMessageConverter()
        {
            MessagePathRegex = new Regex(@"^\/channels\/(?<guild>(?:\d+|@me))\/(?<channel>\d+)\/(?<message>\d+)\/?$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        async Task<Emzi0767.Optional<ITrinityMessage>> IArgumentConverter<ITrinityMessage>.ConvertAsync(string value, CommandContext ctx)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Optional.FromNoValue<ITrinityMessage>();
            if (ctx.Client is TrinityDiscordClient discordClient)
            {
                var msguri = value.StartsWith("<") && value.EndsWith(">") ? value.Substring(1, value.Length - 2) : value;
                ulong mid;
                if (Uri.TryCreate(msguri, UriKind.Absolute, out var uri))
                {
                    if (uri.Host != "discordapp.com" && uri.Host != "discord.com" && !uri.Host.EndsWith(".discordapp.com") && !uri.Host.EndsWith(".discord.com"))
                        return Optional.FromNoValue<ITrinityMessage>();

                    var uripath = MessagePathRegex.Match(uri.AbsolutePath);
                    if (!uripath.Success
                        || !ulong.TryParse(uripath.Groups["channel"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cid)
                        || !ulong.TryParse(uripath.Groups["message"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
                        return Optional.FromNoValue<ITrinityMessage>();

                    var chn = await discordClient.client.GetChannelAsync(cid).ConfigureAwait(false);
                    if (chn == null)
                        return Optional.FromNoValue<ITrinityMessage>();

                    var msg = await chn.GetMessageAsync(mid).ConfigureAwait(false);
                    return msg != null ? Optional.FromValue<ITrinityMessage>(new TrinityDiscordMessage(msg)) : Optional.FromNoValue<ITrinityMessage>();
                }

                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
                {
                    var result = await ((TrinityDiscordChannel)ctx.Channel).x.GetMessageAsync(mid).ConfigureAwait(false);
                    return result != null ? Optional.FromValue<ITrinityMessage>(new TrinityDiscordMessage(result)) : Optional.FromNoValue<ITrinityMessage>();
                }
            }

            return Optional.FromNoValue<ITrinityMessage>();
        }
    }

    public class DiscordMessageConverter : IArgumentConverter<TrinityDiscordMessage>
    {
        private static Regex MessagePathRegex { get; }

        static DiscordMessageConverter()
        {
            MessagePathRegex = new Regex(@"^\/channels\/(?<guild>(?:\d+|@me))\/(?<channel>\d+)\/(?<message>\d+)\/?$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        async Task<Emzi0767.Optional<TrinityDiscordMessage>> IArgumentConverter<TrinityDiscordMessage>.ConvertAsync(string value, CommandContext ctx)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Optional.FromNoValue<TrinityDiscordMessage>();
            if (ctx.Client is TrinityDiscordClient discordClient)
            {
                var msguri = value.StartsWith("<") && value.EndsWith(">") ? value.Substring(1, value.Length - 2) : value;
                ulong mid;
                if (Uri.TryCreate(msguri, UriKind.Absolute, out var uri))
                {
                    if (uri.Host != "discordapp.com" && uri.Host != "discord.com" && !uri.Host.EndsWith(".discordapp.com") && !uri.Host.EndsWith(".discord.com"))
                        return Optional.FromNoValue<TrinityDiscordMessage>();

                    var uripath = MessagePathRegex.Match(uri.AbsolutePath);
                    if (!uripath.Success
                        || !ulong.TryParse(uripath.Groups["channel"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cid)
                        || !ulong.TryParse(uripath.Groups["message"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
                        return Optional.FromNoValue<TrinityDiscordMessage>();

                    var chn = await discordClient.client.GetChannelAsync(cid).ConfigureAwait(false);
                    if (chn == null)
                        return Optional.FromNoValue<TrinityDiscordMessage>();

                    var msg = await chn.GetMessageAsync(mid).ConfigureAwait(false);
                    return msg != null ? Optional.FromValue(new TrinityDiscordMessage(msg)) : Optional.FromNoValue<TrinityDiscordMessage>();
                }

                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out mid))
                {
                    var result = await ((TrinityDiscordChannel)ctx.Channel).x.GetMessageAsync(mid).ConfigureAwait(false);
                    return result != null ? Optional.FromValue(new TrinityDiscordMessage(result)) : Optional.FromNoValue<TrinityDiscordMessage>();
                }
            }

            return Optional.FromNoValue<TrinityDiscordMessage>();
        }
    }

    #endregion DiscordMessage

    #region DiscordGuild

    public class DiscordGuildConverter : IArgumentConverter<TrinityDiscordGuild>
    {
        Task<Emzi0767.Optional<TrinityDiscordGuild>> IArgumentConverter<TrinityDiscordGuild>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Client is TrinityDiscordClient client)
            {
                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gid))
                {
                    return client.client.Guilds.TryGetValue(gid, out var result)
                        ? Task.FromResult(Optional.FromValue(new TrinityDiscordGuild(result)))
                        : Task.FromResult(Optional.FromNoValue<TrinityDiscordGuild>());
                }

                var cs = ctx.Config.CaseSensitive;

                var gld = client.client.Guilds.Values.FirstOrDefault(xg =>
                    xg.Name.Equals(value, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase));
                return Task.FromResult(gld != null ? Optional.FromValue(new TrinityDiscordGuild(gld)) : Optional.FromNoValue<TrinityDiscordGuild>());
            }
            return Task.FromResult(Optional.FromNoValue<TrinityDiscordGuild>());
        }
    }

    public class TrinityDiscordGuildConverter : IArgumentConverter<ITrinityGuild>
    {
        Task<Emzi0767.Optional<ITrinityGuild>> IArgumentConverter<ITrinityGuild>.ConvertAsync(string value, CommandContext ctx)
        {
            if (ctx.Client is TrinityDiscordClient client)
            {
                if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var gid))
                {
                    return client.client.Guilds.TryGetValue(gid, out var result)
                        ? Task.FromResult(Optional.FromValue<ITrinityGuild>(new TrinityDiscordGuild(result)))
                        : Task.FromResult(Optional.FromNoValue<ITrinityGuild>());
                }

                var cs = ctx.Config.CaseSensitive;

                var gld = client.client.Guilds.Values.FirstOrDefault(xg =>
                    xg.Name.Equals(value, cs ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase));
                return Task.FromResult(gld != null ? Optional.FromValue<ITrinityGuild>(new TrinityDiscordGuild(gld)) : Optional.FromNoValue<ITrinityGuild>());
            }
            return Task.FromResult(Optional.FromNoValue<ITrinityGuild>());
        }
    }

    #endregion DiscordGuild

    //TODO
    /*
    public class DiscordChannelConverter : IArgumentConverter<DiscordChannel>
    {
        private static Regex ChannelRegex { get; }

        static DiscordChannelConverter()
        {
            ChannelRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
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
            ThreadRegex = new Regex(@"^<#(\d+)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
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
}*/
}