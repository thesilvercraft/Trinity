namespace Trinity.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string[] CommandNames { get; set; }

    public CommandAttribute(params string[] names)
    {
        CommandNames = names;
    }
}