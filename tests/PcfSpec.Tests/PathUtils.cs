namespace PcfSpec.Tests;

internal static class PathUtils
{
    static PathUtils()
    {
        if (Directory.Exists("temp"))
        {
            Directory.Delete("temp", true);
        }
    }

    public static string CreateTempDir()
    {
        var tempDir = Path.Combine("temp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }
}
