namespace Trinity.Commands;

public class CommandsModule
{
    public virtual Task BeforeCommandExecution(CommandContext command) => Task.CompletedTask;

    public virtual Task AfterCommandExecution(CommandContext command) => Task.CompletedTask;
}