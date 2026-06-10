using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfGlyphNamesTests
{
    [Fact]
    public void TestCopy()
    {
        var names1 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var names2 = names1.Copy();

        Assert.Equal(names1, names2);
        Assert.NotSame(names1, names2);
        Assert.Same(names1.TableFormat, names2.TableFormat);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var names1 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var names2 = names1.DeepCopy();

        Assert.Equal(names1, names2);
        Assert.NotSame(names1, names2);
        Assert.NotSame(names1.TableFormat, names2.TableFormat);
    }

    [Fact]
    public void TestEquals()
    {
        var names1 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var names2 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        Assert.Equal(names1, names2);
    }
}
