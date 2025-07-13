namespace PcfSpec.Tests;

public class LoadSaveTests
{
    [Fact]
    public void TestUnifont()
    {
        var loadPath = Path.Combine("assets", "unifont", "unifont-17.0.03.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "unifont-17.0.03.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_5_8()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-5x8.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-5x8.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_6_12()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-6x12.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-6x12.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_8_16()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-8x16.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-8x16.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_12_24()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-12x24.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-12x24.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_16_32()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-16x32.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-16x32.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestSpleen_32_64()
    {
        var loadPath = Path.Combine("assets", "spleen", "spleen-32x64.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "spleen-32x64.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoLsbyteLsbitP4U2()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p4-u2.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-lsbyte-lsbit-p4-u2.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoLsbyteMsbitP4U2()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-lsbyte-msbit-p4-u2.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-lsbyte-msbit-p4-u2.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoMsbyteLsbitP4U2()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-msbyte-lsbit-p4-u2.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-msbyte-lsbit-p4-u2.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoMsbyteMsbitP4U2()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-msbyte-msbit-p4-u2.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-msbyte-msbit-p4-u2.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoLsbyteLsbitP2U4()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-lsbyte-lsbit-p2-u4.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-lsbyte-lsbit-p2-u4.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoLsbyteMsbitP2U4()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-lsbyte-msbit-p2-u4.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-lsbyte-msbit-p2-u4.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoMsbyteLsbitP2U4()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-msbyte-lsbit-p2-u4.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-msbyte-lsbit-p2-u4.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemoMsbyteMsbitP2U4()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-msbyte-msbit-p2-u4.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-msbyte-msbit-p2-u4.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemo()
    {
        var loadPath = Path.Combine("assets", "demo", "demo.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }

    [Fact]
    public void TestDemo2()
    {
        var loadPath = Path.Combine("assets", "demo", "demo-2.pcf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "demo-2.pcf");
        var font = PcfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllBytes(loadPath), File.ReadAllBytes(savePath));
    }
}
