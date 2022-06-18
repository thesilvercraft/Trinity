// This file is part of the DSharpPlus project.
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.Commands.Attributes;
using Trinity.Commands.Builders;
using Trinity.Commands.Converters;
using Trinity.Commands.Entities;
using Trinity.Commands.Exceptions;
using Trinity.Commands.Executors;
using Trinity.Shared;
using Emzi0767.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Trinity.Commands
{
    /// <summary>
    /// This is the class which handles command registration, management, and execution.
    /// </summary>
    public class CommandsNextExtension : TrinityBaseExtension, IDisposable
    {
        private CommandsNextConfiguration Config { get; }
        private HelpFormatterFactory HelpFormatter { get; }

        private MethodInfo ConvertGeneric { get; }
        private Dictionary<Type, Dictionary<Type, string>> UserFriendlyTypeNames { get; }
        internal Dictionary<Type, Dictionary<Type, IArgumentConverter>> ArgumentConverters { get; }

        internal CultureInfo DefaultParserCulture
            => Config.DefaultParserCulture;

        /// <summary>
        /// Gets the service provider this CommandsNext module was configured with.
        /// </summary>
        public IServiceProvider Services
            => Config.Services;

        public CommandsNextExtension(CommandsNextConfiguration cfg)
        {
            Config = new CommandsNextConfiguration(cfg);
            TopLevelCommands = new Dictionary<string, Command>();
            _registeredCommandsLazy = new Lazy<IReadOnlyDictionary<string, Command>>(() => new ReadOnlyDictionary<string, Command>(TopLevelCommands));
            HelpFormatter = new HelpFormatterFactory();
            HelpFormatter.SetFormatterType<DefaultHelpFormatter>();

            ArgumentConverters = new Dictionary<Type, Dictionary<Type, IArgumentConverter>>
            { {typeof(IPlatformProvider), new Dictionary<Type, IArgumentConverter>
                {
                [typeof(string)] = new StringConverter(),
                [typeof(bool)] = new BoolConverter(),
                [typeof(sbyte)] = new Int8Converter(),
                [typeof(byte)] = new Uint8Converter(),
                [typeof(short)] = new Int16Converter(),
                [typeof(ushort)] = new Uint16Converter(),
                [typeof(int)] = new Int32Converter(),
                [typeof(uint)] = new Uint32Converter(),
                [typeof(long)] = new Int64Converter(),
                [typeof(ulong)] = new Uint64Converter(),
                [typeof(float)] = new Float32Converter(),
                [typeof(double)] = new Float64Converter(),
                [typeof(decimal)] = new Float128Converter(),
                [typeof(DateTime)] = new DateTimeConverter(),
                [typeof(DateTimeOffset)] = new DateTimeOffsetConverter(),
                [typeof(TimeSpan)] = new TimeSpanConverter(),
                [typeof(Uri)] = new UriConverter(),
                [typeof(TrinityColor)] = new TrinityColorConverter()
                }}
            };

            UserFriendlyTypeNames = new Dictionary<Type, Dictionary<Type, string>>()
            {
                {typeof(IPlatformProvider), new Dictionary<Type, string>
                {
                    [typeof(string)] = "string (text, seperate strings by using quotatin marks, example: examplecommand \"1\" \"2\" \"3\" \"etc\")",
                    [typeof(bool)] = "boolean (true(yes) or false(no))",
                    [typeof(sbyte)] = $"signed byte ({sbyte.MinValue} to {sbyte.MaxValue})",
                    [typeof(byte)] = $"byte ({byte.MinValue} to {byte.MaxValue})",
                    [typeof(short)] = $"short ({short.MinValue} to {short.MaxValue})",
                    [typeof(ushort)] = $"unsigned short ({ushort.MinValue} to {ushort.MaxValue})",
                    [typeof(int)] = $"int ({int.MinValue} to {int.MaxValue})",
                    [typeof(uint)] = $"unsigned int ({uint.MinValue} to {uint.MaxValue})",
                    [typeof(long)] = $"long ({long.MinValue} to {long.MaxValue})",
                    [typeof(ulong)] = $"unsigned long ({ulong.MinValue} to {ulong.MinValue})",
                    [typeof(float)] = $"float ({float.MinValue} to {float.MaxValue})",
                    [typeof(double)] = $"double ({double.MinValue} to {double.MaxValue}, supports + and - infinity)",
                    [typeof(decimal)] = $"decimal ({decimal.MinValue} to {decimal.MaxValue})",
                    [typeof(DateTime)] = "date and time",
                    [typeof(DateTimeOffset)] = "date and time",
                    [typeof(TimeSpan)] = "time span (duration, example: 1h30m)",
                    [typeof(Uri)] = "URL (link, example: https://example.com)",
                    [typeof(ITrinityUser)] = "user",
                    [typeof(ITrinityChannel)] = "channel",
                    [typeof(ITrinityGuild)] = "guild",
                    [typeof(ITrinityMessage)] = "message",
                    [typeof(TrinityColor)] = "color"
                }}
            };

            var ncvt = typeof(NullableConverter<>);
            var nt = typeof(Nullable<>);
            var cvts = ArgumentConverters.Keys.ToArray();

            foreach (var xt in cvts)
            {
                var cvtsr = ArgumentConverters[xt].Keys.ToArray();
                foreach (var xtr in cvtsr)
                {
                    var xti = xtr.GetTypeInfo();
                    if (!xti.IsValueType)
                        continue;

                    var xcvt = ncvt.MakeGenericType(xtr);
                    var xnt = nt.MakeGenericType(xtr);

                    if (ArgumentConverters.ContainsKey(xcvt) || Activator.CreateInstance(xcvt) is not IArgumentConverter xcv)
                        continue;

                    ArgumentConverters[xt][xnt] = xcv;
                    UserFriendlyTypeNames[xt][xnt] = UserFriendlyTypeNames[xt][xtr];
                }
            }

            var t = typeof(CommandsNextExtension);
            var ms = t.GetTypeInfo().DeclaredMethods;
            var m = ms.FirstOrDefault(xm => xm.Name == nameof(ConvertArgument) && xm.ContainsGenericParameters && !xm.IsStatic && xm.IsPublic);
            ConvertGeneric = m;
        }

        /// <summary>
        /// Sets the help formatter to use with the default help command.
        /// </summary>
        /// <typeparam name="T">Type of the formatter to use.</typeparam>
        public void SetHelpFormatter<T>() where T : BaseHelpFormatter => HelpFormatter.SetFormatterType<T>();

        /// <summary>
        /// Disposes of this the resources used by CNext.
        /// </summary>
        public void Dispose()
            => Config.CommandExecutor.Dispose();

        #region DiscordClient Registration

        private ILogger Logger;

        public void SetupEventHandlers(AsyncEventExceptionHandler<CommandsNextExtension, CommandExecutionEventArgs> CommandExecutionEventErrorHandler, AsyncEventExceptionHandler<CommandsNextExtension, CommandErrorEventArgs> CommandErrorEventErrorHandler)
        {
            _executed = new Emzi0767.Utilities.AsyncEvent<CommandsNextExtension, CommandExecutionEventArgs>("COMMAND_EXECUTED", TimeSpan.Zero, CommandExecutionEventErrorHandler);
            _error = new Emzi0767.Utilities.AsyncEvent<CommandsNextExtension, CommandErrorEventArgs>("COMMAND_ERRORED", TimeSpan.Zero, CommandErrorEventErrorHandler);
        }

        /// <summary>
        /// DO NOT USE THIS MANUALLY.
        /// </summary>
        /// <param name="client">DO NOT USE THIS MANUALLY.</param>
        /// <exception cref="InvalidOperationException"/>
        public override void Setup(IPlatformProvider client)
        {
            Logger = new GamingLogger();

            if (Config.UseDefaultCommandHandler)
                client.MessageRecieved += this.HandleCommandsAsync;
            else
                Logger.LogWarning(CommandsNextEvents.Misc, "Not attaching default command handler - if this is intentional, you can ignore this message");

            if (Config.EnableDefaultHelp)
            {
                RegisterCommands(typeof(DefaultHelpModule), null, Enumerable.Empty<CheckBaseAttribute>(), out var tcmds);

                if (Config.DefaultHelpChecks.Any())
                {
                    var checks = Config.DefaultHelpChecks.ToArray();

                    for (var i = 0; i < tcmds.Count; i++)
                        tcmds[i].WithExecutionChecks(checks);
                }

                if (tcmds != null)
                    foreach (var xc in tcmds)
                        AddToCommandDictionary(xc.Build(null));
                Config.EnableDefaultHelp = false;
            }

            if (Config.CommandExecutor is ParallelQueuedCommandExecutor pqce)
                Logger.LogDebug(CommandsNextEvents.Misc, "Using parallel executor with degree {0}", pqce.Parallelism);
        }

        #endregion DiscordClient Registration

        #region Command Handling

        private async Task HandleCommandsAsync(IPlatformProvider sender, MessageCreatedEventArgs e)
        {
            if (e.Author.IsAutomated) // bad bot
                return;

            if (!Config.EnableDms && e.Channel.IsPrivate)
                return;

            var mpos = -1;
            if (Config.EnableMentionPrefix)
                mpos = e.Message.GetMentionPrefixLength(sender.CurrentUser);

            if (Config.StringPrefixes.Any())
                foreach (var pfix in Config.StringPrefixes)
                    if (mpos == -1 && !string.IsNullOrWhiteSpace(pfix))
                        mpos = e.Message.GetStringPrefixLength(pfix, Config.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

            if (mpos == -1 && Config.PrefixResolver != null)
                mpos = await Config.PrefixResolver(e.Message).ConfigureAwait(false);

            if (mpos == -1)
                return;

            var pfx = e.Message.PlainTextMessage.Substring(0, mpos);
            var cnt = e.Message.PlainTextMessage.Substring(mpos);

            var __ = 0;
            var fname = cnt.ExtractNextArgument(ref __, Config.QuotationMarks);

            var cmd = this.FindCommand(cnt, out var args);
            var ctx = this.CreateContext(sender, e.Message, pfx, cmd, args);

            if (cmd is null)
            {
                await _error.InvokeAsync(this, new CommandErrorEventArgs { Context = ctx, Exception = new CommandNotFoundException(fname ?? "UnknownCmd") }).ConfigureAwait(false);
                return;
            }

            await Config.CommandExecutor.ExecuteAsync(ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds a specified command by its qualified name, then separates arguments.
        /// </summary>
        /// <param name="commandString">Qualified name of the command, optionally with arguments.</param>
        /// <param name="rawArguments">Separated arguments.</param>
        /// <returns>Found command or null if none was found.</returns>
        public Command? FindCommand(string commandString, out string? rawArguments)
        {
            rawArguments = null;

            var ignoreCase = !Config.CaseSensitive;
            var pos = 0;
            var next = commandString.ExtractNextArgument(ref pos, Config.QuotationMarks);
            if (next is null)
                return null;

            if (!RegisteredCommands.TryGetValue(next, out var cmd))
            {
                if (!ignoreCase)
                    return null;

                var cmdKvp = RegisteredCommands.FirstOrDefault(x => x.Key.Equals(next, StringComparison.InvariantCultureIgnoreCase));
                if (cmdKvp.Value is null)
                    return null;

                cmd = cmdKvp.Value;
            }

            if (cmd is not CommandGroup)
            {
                rawArguments = commandString.Substring(pos).Trim();
                return cmd;
            }

            while (cmd is CommandGroup)
            {
                var cm2 = cmd as CommandGroup;
                var oldPos = pos;
                next = commandString.ExtractNextArgument(ref pos, Config.QuotationMarks);
                if (next is null)
                    break;

                var comparison = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
                cmd = cm2?.Children.FirstOrDefault(x => x.Name.Equals(next, comparison) || x.Aliases.Any(xx => xx.Equals(next, comparison)));

                if (cmd is null)
                {
                    cmd = cm2;
                    pos = oldPos;
                    break;
                }
            }

            rawArguments = commandString.Substring(pos).Trim();
            return cmd;
        }

        /// <summary>
        /// Creates a command execution context from specified arguments.
        /// </summary>
        /// <param name="msg">Message to use for context.</param>
        /// <param name="prefix">Command prefix, used to execute commands.</param>
        /// <param name="cmd">Command to execute.</param>
        /// <param name="rawArguments">Raw arguments to pass to command.</param>
        /// <returns>Created command execution context.</returns>
        public CommandContext CreateContext(IPlatformProvider client, ITrinityMessage msg, string prefix, Command? cmd, string? rawArguments = null)
        {
            var ctx = new CommandContext
            {
                Client = client,
                Command = cmd,
                Message = msg,
                Config = Config,
                RawArgumentString = rawArguments ?? "",
                Prefix = prefix,
                CommandsNext = this,
                Services = Services
            };

            if (cmd is not null && (cmd.Module is TransientCommandModule || cmd.Module == null))
            {
                var scope = ctx.Services.CreateScope();
                ctx.ServiceScopeContext = new CommandContext.ServiceContext(ctx.Services, scope);
                ctx.Services = scope.ServiceProvider;
            }

            return ctx;
        }

        /// <summary>
        /// Executes specified command from given context.
        /// </summary>
        /// <param name="ctx">Context to execute command from.</param>
        /// <returns></returns>
        public async Task ExecuteCommandAsync(CommandContext ctx)
        {
            try
            {
                var cmd = ctx.Command;

                if (cmd is null)
                    return;

                await RunAllChecksAsync(cmd, ctx).ConfigureAwait(false);

                var res = await cmd.ExecuteAsync(ctx).ConfigureAwait(false);

                if (res.IsSuccessful)
                    await _executed.InvokeAsync(this, new CommandExecutionEventArgs { Context = res.Context }).ConfigureAwait(false);
                else
                    await _error.InvokeAsync(this, new CommandErrorEventArgs { Context = res.Context, Exception = res.Exception }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _error.InvokeAsync(this, new CommandErrorEventArgs { Context = ctx, Exception = ex }).ConfigureAwait(false);
            }
            finally
            {
                if (ctx.ServiceScopeContext.IsInitialized)
                    ctx.ServiceScopeContext.Dispose();
            }
        }

        private async Task RunAllChecksAsync(Command cmd, CommandContext ctx)
        {
            if (cmd.Parent is not null)
                await RunAllChecksAsync(cmd.Parent, ctx).ConfigureAwait(false);

            var fchecks = await cmd.RunChecksAsync(ctx, false).ConfigureAwait(false);
            if (fchecks.Any())
                throw new ChecksFailedException(cmd, ctx, fchecks);
        }

        #endregion Command Handling

        #region Command Registration

        /// <summary>
        /// Gets a dictionary of registered top-level commands.
        /// </summary>
        public IReadOnlyDictionary<string, Command> RegisteredCommands
            => _registeredCommandsLazy.Value;

        private Dictionary<string, Command> TopLevelCommands { get; set; }
        private readonly Lazy<IReadOnlyDictionary<string, Command>> _registeredCommandsLazy;

        /// <summary>
        /// Registers all commands from a given assembly. The command classes need to be public to be considered for registration.
        /// </summary>
        /// <param name="assembly">Assembly to register commands from.</param>
        public void RegisterCommands(Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(xt =>
            {
                var xti = xt.GetTypeInfo();
                return xti.IsModuleCandidateType() && !xti.IsNested;
            });
            foreach (var xt in types)
                RegisterCommands(xt);
        }

        /// <summary>
        /// Registers all commands from a given command class.
        /// </summary>
        /// <typeparam name="T">Class which holds commands to register.</typeparam>
        public void RegisterCommands<T>() where T : BaseCommandModule
        {
            var t = typeof(T);
            RegisterCommands(t);
        }

        /// <summary>
        /// Registers all commands from a given command class.
        /// </summary>
        /// <param name="t">Type of the class which holds commands to register.</param>
        public void RegisterCommands(Type t)
        {
            if (t is null)
                throw new ArgumentNullException(nameof(t), "Type cannot be null.");

            if (!t.IsModuleCandidateType())
                throw new ArgumentNullException(nameof(t), "Type must be a class, which cannot be abstract or static.");

            RegisterCommands(t, null, Enumerable.Empty<CheckBaseAttribute>(), out var tempCommands);

            if (tempCommands != null)
                foreach (var command in tempCommands)
                    AddToCommandDictionary(command.Build(null));
        }

        private void RegisterCommands(Type t, CommandGroupBuilder? currentParent, IEnumerable<CheckBaseAttribute> inheritedChecks, out List<CommandBuilder> foundCommands)
        {
            var ti = t.GetTypeInfo();

            var lifespan = ti.GetCustomAttribute<ModuleLifespanAttribute>();
            var moduleLifespan = lifespan != null ? lifespan.Lifespan : ModuleLifespan.Singleton;

            var module = new CommandModuleBuilder()
                .WithType(t)
                .WithLifespan(moduleLifespan)
                .Build(Services);

            // restrict parent lifespan to more or equally restrictive
            if (currentParent?.Module is TransientCommandModule && moduleLifespan != ModuleLifespan.Transient)
                throw new InvalidOperationException("In a transient module, child modules can only be transient.");

            // check if we are anything
            var groupBuilder = new CommandGroupBuilder(module);
            var isModule = false;
            var moduleAttributes = ti.GetCustomAttributes();
            var moduleHidden = false;
            var moduleChecks = new List<CheckBaseAttribute>();

            foreach (var xa in moduleAttributes)
            {
                switch (xa)
                {
                    case GroupAttribute g:
                        isModule = true;
                        var moduleName = g.Name;
                        if (moduleName is null)
                        {
                            moduleName = ti.Name;

                            if (moduleName.EndsWith("Group") && moduleName != "Group")
                                moduleName = moduleName.Substring(0, moduleName.Length - 5);
                            else if (moduleName.EndsWith("Module") && moduleName != "Module")
                                moduleName = moduleName.Substring(0, moduleName.Length - 6);
                            else if (moduleName.EndsWith("Commands") && moduleName != "Commands")
                                moduleName = moduleName.Substring(0, moduleName.Length - 8);
                        }

                        if (!Config.CaseSensitive)
                            moduleName = moduleName.ToLowerInvariant();

                        groupBuilder.WithName(moduleName);

                        foreach (var chk in inheritedChecks)
                            groupBuilder.WithExecutionCheck(chk);

                        foreach (var mi in ti.DeclaredMethods.Where(x => x.IsCommandCandidate(out _) && x.GetCustomAttribute<GroupCommandAttribute>() != null))
                            groupBuilder.WithOverload(new CommandOverloadBuilder(mi));
                        break;

                    case AliasesAttribute a:
                        foreach (var xalias in a.Aliases)
                            groupBuilder.WithAlias(Config.CaseSensitive ? xalias : xalias.ToLowerInvariant());
                        break;

                    case HiddenAttribute h:
                        groupBuilder.WithHiddenStatus(true);
                        moduleHidden = true;
                        break;

                    case DescriptionAttribute d:
                        groupBuilder.WithDescription(d.Description);
                        break;

                    case CheckBaseAttribute c:
                        moduleChecks.Add(c);
                        groupBuilder.WithExecutionCheck(c);
                        break;

                    default:
                        groupBuilder.WithCustomAttribute(xa);
                        break;
                }
            }

            if (!isModule)
            {
                groupBuilder = null;
                if (!inheritedChecks.Any())
                    moduleChecks.AddRange(inheritedChecks);
            }

            // candidate methods
            var methods = ti.DeclaredMethods;
            var commands = new List<CommandBuilder>();
            var commandBuilders = new Dictionary<string, CommandBuilder>();
            foreach (var m in methods)
            {
                if (!m.IsCommandCandidate(out _))
                    continue;

                var attrs = m.GetCustomAttributes();
                if (attrs.FirstOrDefault(xa => xa is CommandAttribute) is not CommandAttribute cattr)
                    continue;

                var commandName = cattr.Name;
                if (commandName is null)
                {
                    commandName = m.Name;
                    if (commandName.EndsWith("Async") && commandName != "Async")
                        commandName = commandName.Substring(0, commandName.Length - 5);
                }

                if (!Config.CaseSensitive)
                    commandName = commandName.ToLowerInvariant();

                if (!commandBuilders.TryGetValue(commandName, out var commandBuilder))
                {
                    commandBuilders.Add(commandName, commandBuilder = new CommandBuilder(module).WithName(commandName));

                    if (!isModule)
                        if (currentParent != null)
                            currentParent.WithChild(commandBuilder);
                        else
                            commands.Add(commandBuilder);
                    else
                        groupBuilder?.WithChild(commandBuilder);
                }

                commandBuilder.WithOverload(new CommandOverloadBuilder(m));

                if (!isModule && moduleChecks.Any())
                    foreach (var chk in moduleChecks)
                        commandBuilder.WithExecutionCheck(chk);

                foreach (var xa in attrs)
                {
                    switch (xa)
                    {
                        case AliasesAttribute a:
                            foreach (var xalias in a.Aliases)
                                commandBuilder.WithAlias(Config.CaseSensitive ? xalias : xalias.ToLowerInvariant());
                            break;

                        case CheckBaseAttribute p:
                            commandBuilder.WithExecutionCheck(p);
                            break;

                        case DescriptionAttribute d:
                            commandBuilder.WithDescription(d.Description);
                            break;

                        case HiddenAttribute h:
                            commandBuilder.WithHiddenStatus(true);
                            break;

                        default:
                            commandBuilder.WithCustomAttribute(xa);
                            break;
                    }
                }

                if (!isModule && moduleHidden)
                    commandBuilder.WithHiddenStatus(true);
            }

            // candidate types
            var types = ti.DeclaredNestedTypes
                .Where(xt => xt.IsModuleCandidateType() && xt.DeclaredConstructors.Any(xc => xc.IsPublic));
            foreach (var type in types)
            {
                RegisterCommands(type.AsType(),
                    groupBuilder,
                    !isModule ? moduleChecks : Enumerable.Empty<CheckBaseAttribute>(),
                    out var tempCommands);

                if (isModule && groupBuilder is not null)
                    foreach (var chk in moduleChecks)
                        groupBuilder.WithExecutionCheck(chk);

                if (isModule && tempCommands is not null && groupBuilder is not null)
                    foreach (var xtcmd in tempCommands)
                        groupBuilder.WithChild(xtcmd);
                else if (tempCommands != null)
                    commands.AddRange(tempCommands);
            }

            if (isModule && currentParent is null && groupBuilder is not null)
                commands.Add(groupBuilder);
            else if (isModule && currentParent is not null && groupBuilder is not null)
                currentParent.WithChild(groupBuilder);
            foundCommands = commands;
        }

        /// <summary>
        /// Builds and registers all supplied commands.
        /// </summary>
        /// <param name="cmds">Commands to build and register.</param>
        public void RegisterCommands(params CommandBuilder[] cmds)
        {
            foreach (var cmd in cmds)
                AddToCommandDictionary(cmd.Build(null));
        }

        /// <summary>
        /// Unregisters specified commands from CommandsNext.
        /// </summary>
        /// <param name="cmds">Commands to unregister.</param>
        public void UnregisterCommands(params Command[] cmds)
        {
            if (cmds.Any(x => x.Parent is not null))
                throw new InvalidOperationException("Cannot unregister nested commands.");

            var keys = RegisteredCommands.Where(x => cmds.Contains(x.Value)).Select(x => x.Key).ToList();
            foreach (var key in keys)
                TopLevelCommands.Remove(key);
        }

        private void AddToCommandDictionary(Command cmd)
        {
            if (cmd.Parent is not null)
                return;

            if (TopLevelCommands.ContainsKey(cmd.Name) || cmd.Aliases.Any(xs => TopLevelCommands.ContainsKey(xs)))
                throw new DuplicateCommandException(cmd.QualifiedName);

            TopLevelCommands[cmd.Name] = cmd;

            foreach (var xs in cmd.Aliases)
                TopLevelCommands[xs] = cmd;
        }

        #endregion Command Registration

        #region Default Help

        [ModuleLifespan(ModuleLifespan.Transient)]
        public class DefaultHelpModule : BaseCommandModule
        {
            [Command("help"), Description("Displays command help.")]
            public async Task DefaultHelpAsync(CommandContext ctx, [Description("Command to provide help for.")] params string[] command)
            {
                var topLevel = ctx.CommandsNext.TopLevelCommands.Values.Distinct();
                var helpBuilder = ctx.CommandsNext.HelpFormatter.Create(ctx);

                if (command != null && command.Any())
                {
                    Command? cmd = null;
                    var searchIn = topLevel;
                    foreach (var c in command)
                    {
                        if (searchIn is null)
                        {
                            cmd = null;
                            break;
                        }

                        var (comparison, comparer) = ctx.Config.CaseSensitive switch
                        {
                            true => (StringComparison.InvariantCulture, StringComparer.InvariantCulture),
                            false => (StringComparison.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase)
                        };
                        cmd = searchIn.FirstOrDefault(xc => xc.Name.Equals(c, comparison) || xc.Aliases.Contains(c, comparer));

                        if (cmd is null)
                            break;

                        var failedChecks = await cmd.RunChecksAsync(ctx, true).ConfigureAwait(false);
                        if (failedChecks.Any())
                            throw new ChecksFailedException(cmd, ctx, failedChecks);

                        searchIn = cmd is CommandGroup cmdGroup ? cmdGroup.Children : null;
                    }

                    if (cmd is null)
                        throw new CommandNotFoundException(string.Join(" ", command));

                    helpBuilder.WithCommand(cmd);

                    if (cmd is CommandGroup group)
                    {
                        var commandsToSearch = group.Children.Where(xc => !xc.IsHidden);
                        var eligibleCommands = new List<Command>();
                        foreach (var candidateCommand in commandsToSearch)
                        {
                            if (candidateCommand.ExecutionChecks == null || !candidateCommand.ExecutionChecks.Any())
                            {
                                eligibleCommands.Add(candidateCommand);
                                continue;
                            }

                            var candidateFailedChecks = await candidateCommand.RunChecksAsync(ctx, true).ConfigureAwait(false);
                            if (!candidateFailedChecks.Any())
                                eligibleCommands.Add(candidateCommand);
                        }

                        if (eligibleCommands.Any())
                            helpBuilder.WithSubcommands(eligibleCommands.OrderBy(xc => xc.Name));
                    }
                }
                else
                {
                    var commandsToSearch = topLevel.Where(xc => !xc.IsHidden);
                    var eligibleCommands = new List<Command>();
                    foreach (var sc in commandsToSearch)
                    {
                        if (sc.ExecutionChecks == null || !sc.ExecutionChecks.Any())
                        {
                            eligibleCommands.Add(sc);
                            continue;
                        }

                        var candidateFailedChecks = await sc.RunChecksAsync(ctx, true).ConfigureAwait(false);
                        if (!candidateFailedChecks.Any())
                            eligibleCommands.Add(sc);
                    }

                    if (eligibleCommands.Any())
                        helpBuilder.WithSubcommands(eligibleCommands.OrderBy(xc => xc.Name));
                }

                var helpMessage = helpBuilder.Build();

                var builder = new TrinityMessageBuilder().WithContent(helpMessage.Content).WithEmbed(helpMessage.Embed);

                await ctx.RespondAsync(builder).ConfigureAwait(false);
            }
        }

        #endregion Default Help

        #region Sudo

        /*/// <summary>
        /// Creates a fake command context to execute commands with.
        /// </summary>
        /// <param name="actor">The user or member to use as message author.</param>
        /// <param name="channel">The channel the message is supposed to appear from.</param>
        /// <param name="messageContents">Contents of the message.</param>
        /// <param name="prefix">Command prefix, used to execute commands.</param>
        /// <param name="cmd">Command to execute.</param>
        /// <param name="rawArguments">Raw arguments to pass to command.</param>
        /// <returns>Created fake context.</returns>
        public CommandContext CreateFakeContext(ITrinityUser actor, ITrinityChannel channel, IPlatformProvider client, string messageContents, string prefix, Command cmd, string? rawArguments = null)
        {
            var epoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var now = DateTimeOffset.UtcNow;
            var timeSpan = (ulong)(now - epoch).TotalMilliseconds;

            // create fake message
            var msg = new ITrinityMessage
            {
                Discord = client,
                Author = actor,
                ChannelId = channel.Id,
                Content = messageContents,
                Id = timeSpan << 22,
                Pinned = false,
                MentionEveryone = messageContents.Contains("@everyone"),
                IsTTS = false,
                _attachments = new List<DiscordAttachment>(),
                _embeds = new List<DiscordEmbed>(),
                TimestampRaw = now,
                _reactions = new List<DiscordReaction>()
            };

            var mentionedUsers = new List<DiscordUser>();
            var mentionedRoles = msg.Channel.Guild != null ? new List<DiscordRole>() : null;
            var mentionedChannels = msg.Channel.Guild != null ? new List<DiscordChannel>() : null;

            if (!string.IsNullOrWhiteSpace(msg.Content))
            {
                if (msg.Channel.Guild != null)
                {
                    mentionedUsers = Utilities.GetUserMentions(msg).Select(xid => msg.Channel.Guild._members.TryGetValue(xid, out var member) ? member : null).Cast<DiscordUser>().ToList();
                    mentionedRoles = Utilities.GetRoleMentions(msg).Select(xid => msg.Channel.Guild.GetRole(xid)).ToList();
                    mentionedChannels = Utilities.GetChannelMentions(msg).Select(xid => msg.Channel.Guild.GetChannel(xid)).ToList();
                }
                else
                {
                    mentionedUsers = Utilities.GetUserMentions(msg).Select(this.Client.GetCachedOrEmptyUserInternal).ToList();
                }
            }

            msg._mentionedUsers = mentionedUsers;
            msg._mentionedRoles = mentionedRoles;
            msg._mentionedChannels = mentionedChannels;

            var ctx = new CommandContext
            {
                Client = this.Client,
                Command = cmd,
                Message = msg,
                Config = Config,
                RawArgumentString = rawArguments ?? "",
                Prefix = prefix,
                CommandsNext = this,
                Services = Services
            };

            if (cmd is not null && (cmd.Module is TransientCommandModule || cmd.Module is null))
            {
                var scope = ctx.Services.CreateScope();
                ctx.ServiceScopeContext = new CommandContext.ServiceContext(ctx.Services, scope);
                ctx.Services = scope.ServiceProvider;
            }

            return ctx;
        }
        */

        #endregion Sudo

        #region Type Conversion

        /// <summary>
        /// Converts a string to specified type.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="value">Value to convert.</param>
        /// <param name="ctx">Context in which to convert to.</param>
        /// <returns>Converted object.</returns>
        public async Task<object> ConvertArgument<T>(string value, CommandContext ctx)
        {
            var t = typeof(T);

            if (ArgumentConverters.ContainsKey(ctx.Client.GetType()))
            {
                var ct = ctx.Client.GetType();
                if (ArgumentConverters[ct].ContainsKey(t))
                {
                    if (ArgumentConverters[ct][t] is IArgumentConverter<T> cv2)
                    {
                        var cvr2 = await cv2.ConvertAsync(value, ctx).ConfigureAwait(false);
                        return !cvr2.HasValue ? throw new ArgumentException("Could not convert specified value to given type.", nameof(value)) : cvr2.Value!;
                    }
                }
            }
            if (!ArgumentConverters[typeof(IPlatformProvider)].ContainsKey(t))
                throw new ArgumentException("There is no converter specified for given type.", nameof(T));

            if (ArgumentConverters[typeof(IPlatformProvider)][t] is not IArgumentConverter<T> cv)
                throw new ArgumentException("Invalid converter registered for this type.", nameof(T));

            var cvr = await cv.ConvertAsync(value, ctx).ConfigureAwait(false);
            return !cvr.HasValue ? throw new ArgumentException("Could not convert specified value to given type.", nameof(value)) : cvr.Value!;
        }

        /// <summary>
        /// Converts a string to specified type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="ctx">Context in which to convert to.</param>
        /// <param name="type">Type to convert to.</param>
        /// <returns>Converted object.</returns>
        public async Task<object> ConvertArgument(string? value, CommandContext ctx, Type type)
        {
            var m = ConvertGeneric.MakeGenericMethod(type);
            try
            {
                return await ((Task<object>)m.Invoke(this, new object?[] { value, ctx })).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is TargetInvocationException or InvalidCastException)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Registers an argument converter for specified type.
        /// </summary>
        /// <typeparam name="T">Type for which to register the converter.</typeparam>
        /// <param name="converter">Converter to register.</param>
        public void RegisterConverter<T>(Type clienttype, IArgumentConverter<T> converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter), "Converter cannot be null.");
            if (clienttype != typeof(IPlatformProvider) && !clienttype.GetInterfaces().Contains(typeof(IPlatformProvider)))
            {
                throw new ArgumentException("Client type must be of type IPlatformProvider.", nameof(clienttype));
            }
            var t = typeof(T);
            var ti = t.GetTypeInfo();
            if (!ArgumentConverters.ContainsKey(clienttype))
            {
                ArgumentConverters[clienttype] = new();
            }

            ArgumentConverters[clienttype][t] = converter;

            if (!ti.IsValueType)
                return;

            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(t);
            var nullableType = typeof(Nullable<>).MakeGenericType(t);
            if (ArgumentConverters.ContainsKey(nullableType))
                return;

            var nullableConverter = Activator.CreateInstance(nullableConverterType) as IArgumentConverter;

            if (nullableConverter is not null)
                ArgumentConverters[clienttype][nullableType] = nullableConverter;
        }

        /// <summary>
        /// Unregisters an argument converter for specified type.
        /// </summary>
        /// <typeparam name="T">Type for which to unregister the converter.</typeparam>
        public void UnregisterConverter<T>()
        {
            var t = typeof(T);
            var ti = t.GetTypeInfo();
            if (ArgumentConverters.ContainsKey(t))
                ArgumentConverters.Remove(t);

            if (UserFriendlyTypeNames.ContainsKey(t))
                UserFriendlyTypeNames.Remove(t);

            if (!ti.IsValueType)
                return;

            var nullableType = typeof(Nullable<>).MakeGenericType(t);
            if (!ArgumentConverters.ContainsKey(nullableType))
                return;

            ArgumentConverters.Remove(nullableType);
            UserFriendlyTypeNames.Remove(nullableType);
        }

        /// <summary>
        /// Registers a user-friendly type name.
        /// </summary>
        /// <typeparam name="T">Type to register the name for.</typeparam>
        /// <param name="value">Name to register.</param>
        public void RegisterUserFriendlyTypeName<T>(Type clienttype, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "Name cannot be null or empty.");
            if (clienttype != typeof(IPlatformProvider) && !clienttype.GetInterfaces().Contains(typeof(IPlatformProvider)))
            {
                throw new ArgumentException("Client type must be of type IPlatformProvider.", nameof(clienttype));
            }
            var t = typeof(T);
            var ti = t.GetTypeInfo();
            if (!ArgumentConverters.ContainsKey(clienttype) || !ArgumentConverters[clienttype].ContainsKey(t))
                throw new InvalidOperationException("Cannot register a friendly name for a type which has no associated converter.");
            if (!UserFriendlyTypeNames.ContainsKey(clienttype))
            {
                UserFriendlyTypeNames[clienttype] = new();
            }
            UserFriendlyTypeNames[clienttype][t] = value;

            if (!ti.IsValueType)
                return;

            var nullableType = typeof(Nullable<>).MakeGenericType(t);
            UserFriendlyTypeNames[clienttype][nullableType] = value;
        }

        /// <summary>
        /// Converts a type into user-friendly type name.
        /// </summary>
        /// <param name="t">Type to convert.</param>
        /// <returns>User-friendly type name.</returns>
        public string GetUserFriendlyTypeName(Type? c, Type t)
        {
            if (c != null && UserFriendlyTypeNames.ContainsKey(c) && UserFriendlyTypeNames[c].ContainsKey(t))
                return UserFriendlyTypeNames[c][t];
            if (UserFriendlyTypeNames.ContainsKey(typeof(IPlatformProvider)) && UserFriendlyTypeNames[typeof(IPlatformProvider)].ContainsKey(t))
                return UserFriendlyTypeNames[typeof(IPlatformProvider)][t];
            var ti = t.GetTypeInfo();
            if (ti.IsGenericTypeDefinition && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var tn = ti.GenericTypeArguments[0];
                return UserFriendlyTypeNames.ContainsKey(c) && UserFriendlyTypeNames[c].ContainsKey(tn) ? UserFriendlyTypeNames[c][tn] : (UserFriendlyTypeNames.ContainsKey(typeof(IPlatformProvider)) && UserFriendlyTypeNames[typeof(IPlatformProvider)].ContainsKey(tn) ? UserFriendlyTypeNames[typeof(IPlatformProvider)][tn] : tn.Name);
            }

            return t.Name;
        }

        #endregion Type Conversion

        #region Helpers

        /// <summary>
        /// Gets the configuration-specific string comparer. This returns <see cref="StringComparer.Ordinal"/> or <see cref="StringComparer.OrdinalIgnoreCase"/>,
        /// depending on whether <see cref="CommandsNextConfiguration.CaseSensitive"/> is set to <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        /// <returns>A string comparer.</returns>
        internal IEqualityComparer<string> GetStringComparer()
            => Config.CaseSensitive
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase;

        #endregion Helpers

        #region Events

        /// <summary>
        /// Triggered whenever a command executes successfully.
        /// </summary>
        public event AsyncEventHandler<CommandsNextExtension, CommandExecutionEventArgs> CommandExecuted
        {
            add { _executed.Register(value); }
            remove { _executed.Unregister(value); }
        }

        private Emzi0767.Utilities.AsyncEvent<CommandsNextExtension, CommandExecutionEventArgs> _executed = null!;

        /// <summary>
        /// Triggered whenever a command throws an exception during execution.
        /// </summary>
        public event AsyncEventHandler<CommandsNextExtension, CommandErrorEventArgs> CommandErrored
        {
            add { _error.Register(value); }
            remove { _error.Unregister(value); }
        }

        private Emzi0767.Utilities.AsyncEvent<CommandsNextExtension, CommandErrorEventArgs> _error = null!;

        private Task OnCommandExecuted(CommandExecutionEventArgs e)
            => _executed.InvokeAsync(this, e);

        private Task OnCommandErrored(CommandErrorEventArgs e)
            => _error.InvokeAsync(this, e);

        #endregion Events
    }
}