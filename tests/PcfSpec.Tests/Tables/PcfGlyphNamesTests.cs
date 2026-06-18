using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfGlyphNamesTests
{
    [Fact]
    public void TestCopy()
    {
        var names1 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        var names2 = names1.Copy();
        var names3 = names1.DeepCopy();

        Assert.Equal(names1, names2);
        Assert.Equal(names1, names3);
        Assert.NotSame(names1, names2);
        Assert.NotSame(names1, names3);
    }

    [Fact]
    public void TestEquals()
    {
        var names1 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        var names2 = new PcfGlyphNames(
            ["A", "B", "C"],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        Assert.Equal(names1, names2);
    }
}
