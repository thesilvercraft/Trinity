namespace Trinity.Shared;

public abstract class TrinityCommandModule
{
    /// <summary>
    /// A function to determine if a command module should be loaded.
    /// </summary>
    /// <returns>a boolean stating the answer</returns>
    public virtual Task<bool> CanBeLoaded()
    {
        return Task.FromResult(true);
    }
}