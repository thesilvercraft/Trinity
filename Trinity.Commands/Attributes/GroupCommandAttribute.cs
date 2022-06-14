namespace Trinity.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class GroupCommandAttribute : Attribute
{
    /// <summary>
    /// Marks this method as a group command.
    /// </summary>
    public GroupCommandAttribute()
    {
    }
}