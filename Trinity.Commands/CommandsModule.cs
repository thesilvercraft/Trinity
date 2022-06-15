namespace Trinity.Commands;

public class CommandsModule
{
    public virtual Task BeforeCommandExecution(CommandContext command) => Task.CompletedTask;

    public virtual Task AfterCommandExecution(CommandContext command) => Task.CompletedTask;

    /// <summary>
    /// A function to determine if a command module should be loaded.
    /// </summary>
    /// <returns>a boolean stating the answer</returns>
    public virtual Task<bool> CanBeLoaded()
    {
        return Task.FromResult(true);
    }
}