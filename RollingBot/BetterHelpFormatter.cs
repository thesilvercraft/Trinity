using RollingBot.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Commands;
using Trinity.Commands.Converters;
using Trinity.Commands.Entities;
using Trinity.Shared;

namespace SilverBotDS.Objects.Classes;

public class CustomHelpFormatter : BaseHelpFormatter
{
    /// <summary>
    ///     Creates a new default help formatter.
    /// </summary>
    /// <param name="ctx">Context in which this formatter is being invoked.</param>
    public CustomHelpFormatter(CommandContext ctx)
        : base(ctx)
    {
        EmbedBuilder = new TrinityEmbed
        {
            //    .WithTitle(Lang.HelpCommandHelpString).WithFooter(Lang.RequestedBy + ctx.User.Username,
            // ctx.User.GetAvatarUrl(ImageFormat.Auto));
            Title = "Help",
            Fields = new()
        };
        TextOnly = !ctx.Channel.GetType().GetInterfaces().Any(x => x == typeof(ITrinityChannelWithAdvancedSendingMethodsAndEmbeds));
        _strBuilder = new();
    }

    private bool TextOnly = false;
    public TrinityEmbed EmbedBuilder { get; }
    public StringBuilder _strBuilder;
    private Command Command { get; set; }

    /// <summary>
    ///     Sets the command this help message will be for.
    /// </summary>
    /// <param name="command">Command for which the help message is being produced.</param>
    /// <returns>This help formatter.</returns>
    public override BaseHelpFormatter WithCommand(Command command)
    {
        Command = command;
        if (!TextOnly)
        {
            EmbedBuilder.Description = (
          $"`{command.Name}`: {command.Description ?? "No description"}");
            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
            {
                EmbedBuilder.Description = ($"{EmbedBuilder.Description}\n{"Can be exectuted as a group"}");
            }

            if (command.Aliases?.Any() == true)
            {
                EmbedBuilder.Fields.Add(new("Aliases",
                    string.Join(", ", command.Aliases.Select(x => $"`{x}`")), false));
            }

            if (command.Overloads?.Any() == true)
            {
                var sb = new StringBuilder();
                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority).Select(ovl => ovl.Arguments))
                {
                    sb.Append('`').Append(command.QualifiedName);
                    foreach (var arg in ovl)
                    {
                        sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name)
                            .Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
                    }

                    sb.Append("`\n");
                    foreach (var arg in ovl)
                    {
                        sb.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(Context.Client.GetType(), arg.Type))
                            .Append(")`: ").Append(arg.Description ?? "No description").Append('\n');
                    }

                    sb.Append('\n');
                }

                EmbedBuilder.Fields.Add(new("Arguments", sb.ToString().Trim(), false));
            }
        }
        else
        {
            _strBuilder.AppendLine($"{command.Name} - {command.Description ?? "No description"}");
            if(command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
            {
                _strBuilder.AppendLine("Can be executed as a group");
            }
            if (command.Aliases?.Any() == true)
            {
                _strBuilder.AppendLine("**Aliases**");
                _strBuilder.AppendLine(string.Join(", ", command.Aliases.Select(x => $"`{x}`")));
            }
            if (command.Overloads?.Any() == true)
            {
                _strBuilder.AppendLine("**Arguments**");
                var sb = new StringBuilder();
                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority).Select(ovl => ovl.Arguments))
                {
                    _strBuilder.Append('`').Append(command.QualifiedName);
                    foreach (var arg in ovl)
                    {
                        _strBuilder.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name)
                            .Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
                    }

                    _strBuilder.Append("`\n");
                    foreach (var arg in ovl)
                    {
                        _strBuilder.Append('`').Append(arg.Name).Append(" (").Append(CommandsNext.GetUserFriendlyTypeName(Context.Client.GetType(), arg.Type))
                            .Append(")`: ").Append(arg.Description ?? "No description").Append('\n');
                    }

                    _strBuilder.Append('\n');
                }
            }
        }

        return this;
    }

    /// <summary>
    ///     Sets the subcommands for this command, if applicable. This method will be called with filtered data.
    /// </summary>
    /// <param name="subcommands">Subcommands for this command group.</param>
    /// <returns>This help formatter.</returns>
    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        if (!TextOnly)
        {
            if (Command == null)
            {
                Dictionary<string, HashSet<string>> commands = new();
                foreach (var command in subcommands)
                {
                    if (command.CustomAttributes.Any(x => x.GetType() == typeof(CategoryAttribute)))
                    {
                        foreach (var attribute in command.CustomAttributes.Where(x =>
                                     x.GetType() == typeof(CategoryAttribute)))
                        {
                            foreach (var category in ((CategoryAttribute)attribute).Category)
                            {
                                if (!commands.ContainsKey(category))
                                {
                                    commands.Add(category, new HashSet<string>());
                                }

                                commands[category].Add(command.Name);
                            }
                        }
                    }

                    if (command.Module.ModuleType.GetCustomAttributes(true)
                        .Any(x => x.GetType() == typeof(CategoryAttribute)))
                    {
                        foreach (var attribute in command.Module.ModuleType.GetCustomAttributes(true)
                                     .Where(x => x is CategoryAttribute))
                        {
                            foreach (var category in ((CategoryAttribute)attribute).Category)
                            {
                                if (!commands.ContainsKey(category))
                                {
                                    commands.Add(category, new HashSet<string>());
                                }

                                commands[category].Add(command.Name);
                            }
                        }
                    }
                }

                foreach (var category in commands.Keys)
                {
                    EmbedBuilder.Fields.Add(new(category,
                        string.Join(", ", commands[category].Select(x => $"`{x}`")), false));
                }
            }
            else
            {
                EmbedBuilder.Fields.Add(new TrinityEmbedField("Subcommands",
                    string.Join(", ", subcommands.Select(x => $"`{x}`")), false));
            }
        }
        else
        {
            if(Command==null)
            {
                Dictionary<string, HashSet<string>> commands = new();
                foreach (var command in subcommands)
                {
                    if (command.CustomAttributes.Any(x => x.GetType() == typeof(CategoryAttribute)))
                    {
                        foreach (var attribute in command.CustomAttributes.Where(x =>
                                     x.GetType() == typeof(CategoryAttribute)))
                        {
                            foreach (var category in ((CategoryAttribute)attribute).Category)
                            {
                                if (!commands.ContainsKey(category))
                                {
                                    commands.Add(category, new HashSet<string>());
                                }

                                commands[category].Add(command.Name);
                            }
                        }
                    }

                    if (command.Module.ModuleType.GetCustomAttributes(true)
                        .Any(x => x.GetType() == typeof(CategoryAttribute)))
                    {
                        foreach (var attribute in command.Module.ModuleType.GetCustomAttributes(true)
                                     .Where(x => x is CategoryAttribute))
                        {
                            foreach (var category in ((CategoryAttribute)attribute).Category)
                            {
                                if (!commands.ContainsKey(category))
                                {
                                    commands.Add(category, new HashSet<string>());
                                }

                                commands[category].Add(command.Name);
                            }
                        }
                    }
                }

                foreach (var category in commands.Keys)
                {
                    _strBuilder.AppendLine($"**{category}**");
                    _strBuilder.AppendLine(string.Join(", ", commands[category].Select(x => $"`{x}`")));
                 
                }
            }
            else
            {
                _strBuilder.AppendLine("Subcommands:");
                foreach (var cmd in subcommands)
                {
                    _strBuilder.AppendLine($"{cmd.Name} - {cmd.Description ?? "No description"}");
                }
            }
            
        }

        return this;
    }

    /// <summary>
    ///     Construct the help message.
    /// </summary>
    /// <returns>Data for the help message.</returns>
    public override CommandHelpMessage Build()
    {
        if (!TextOnly)
        {
            if (Command == null)
            {
                EmbedBuilder.Description = ("Listing all commands and groups. Specify a command to see more information.");
            }

            return new CommandHelpMessage(embed: EmbedBuilder);
        }
        else
        {
            return new CommandHelpMessage(content: _strBuilder.ToString());
        }
    }
}