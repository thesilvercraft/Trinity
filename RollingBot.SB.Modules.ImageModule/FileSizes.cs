namespace SilverBotDS.Utils;

internal class FileSizes
{
    public static readonly FSize[] FSizes =
    {
        new("Byte", "B"), new("Kilobyte", "KB"), new("Megabyte", "MB"),
        new("Gigabyte", "GB"), new("Terabyte", "TB"), new("Petabyte", "PB"),
        new("Exabyte", "EB"), new("Zettabyte", "ZB"), new("Yottabyte", "YB")
    };

    protected FileSizes()
    {
    }
}