namespace PcfSpec.Tests.Conformance;

public class ParseDumpTests
{
    [Theory]
    [InlineData("demo", "demo.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p1-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p1-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p1-u4.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p2-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p2-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p2-u4.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p4-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p4-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-lsbit-p4-u4.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p1-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p1-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p1-u4.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p2-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p2-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p2-u4.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p4-u1.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p4-u2.pcf")]
    [InlineData("demo", "demo-lsbyte-msbit-p4-u4.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p1-u1.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p1-u2.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p1-u4.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p2-u1.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p2-u2.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p2-u4.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p4-u1.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p4-u2.pcf")]
    [InlineData("demo", "demo-msbyte-lsbit-p4-u4.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p1-u1.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p1-u2.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p1-u4.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p2-u1.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p2-u2.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p2-u4.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p4-u1.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p4-u2.pcf")]
    [InlineData("demo", "demo-msbyte-msbit-p4-u4.pcf")]
    [InlineData("demo", "demo-2.pcf")]
    [InlineData("spleen", "spleen-5x8.pcf")]
    [InlineData("spleen", "spleen-6x12.pcf")]
    [InlineData("spleen", "spleen-8x16.pcf")]
    [InlineData("spleen", "spleen-12x24.pcf")]
    [InlineData("spleen", "spleen-16x32.pcf")]
    [InlineData("spleen", "spleen-32x64.pcf")]
    [InlineData("unifont", "unifont-17.0.05.pcf")]
    public void TestParseDump(string fontDir, string fontFileName)
    {
        var data = File.ReadAllBytes(Path.Combine("assets", fontDir, fontFileName));
        var font = PcfFont.Parse(data);
        Assert.Equal(data, font.DumpToBytes());
    }
}
