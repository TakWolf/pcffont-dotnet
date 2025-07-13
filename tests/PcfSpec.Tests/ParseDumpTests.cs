namespace PcfSpec.Tests;

public class ParseDumpTests
{
    [Fact]
    public void TestUnifont()
    {
        var data = File.ReadAllBytes(Path.Combine("assets", "unifont", "unifont-16.0.04.pcf"));
        var font = PcfFont.Parse(data);
        Assert.Equal(data, font.DumpToBytes());
    }
}
