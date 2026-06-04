using System.Text.RegularExpressions;
using PcfSpec.Tests.TestUtils;

namespace PcfSpec.Tests.Conformance;

public partial class LoadSaveTests
{
    [Fact]
    public void TestUnifont()
    {
        var loadPath = Path.Combine("assets", "unifont", "unifont-17.0.04.pcf");
        var savePath = Path.Combine(PathUtil.CreateTempDir(), "unifont-17.0.04.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [GeneratedRegex(@"^spleen-.*\.pcf$")]
    private static partial Regex RegexSpleenFontName();

    [Fact]
    public void TestSpleen()
    {
        var loadPaths = Directory.GetFiles(Path.Combine("assets", "spleen"))
            .Where(path => RegexSpleenFontName().IsMatch(Path.GetFileName(path)))
            .Order()
            .ToList();
        Assert.Equal(6, loadPaths.Count);

        foreach (var loadPath in loadPaths)
        {
            var savePath = Path.Combine(PathUtil.CreateTempDir(), Path.GetFileName(loadPath));
            var font = PcfFont.Load(loadPath);
            font.Save(savePath);
            Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
        }
    }

    [GeneratedRegex(@"^demo.*\.pcf$")]
    private static partial Regex RegexDemoFontName();

    [Fact]
    public void TestDemo()
    {
        var loadPaths = Directory.GetFiles(Path.Combine("assets", "demo"))
            .Where(path => RegexDemoFontName().IsMatch(Path.GetFileName(path)))
            .Order()
            .ToList();
        Assert.Equal(38, loadPaths.Count);

        foreach (var loadPath in loadPaths)
        {
            var savePath = Path.Combine(PathUtil.CreateTempDir(), Path.GetFileName(loadPath));
            var font = PcfFont.Load(loadPath);
            font.Save(savePath);
            Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
        }
    }
}
