using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfScalableWidthsTests
{
    [Fact]
    public void TestCopy()
    {
        var scalableWidths1 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        var scalableWidths2 = scalableWidths1.Copy();
        var scalableWidths3 = scalableWidths1.DeepCopy();

        Assert.Equal(scalableWidths1, scalableWidths2);
        Assert.Equal(scalableWidths1, scalableWidths3);
        Assert.NotSame(scalableWidths1, scalableWidths2);
        Assert.NotSame(scalableWidths1, scalableWidths3);
    }

    [Fact]
    public void TestEquals()
    {
        var scalableWidths1 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        var scalableWidths2 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: PcfTableFormat.Of(true, true, true, 2, 4));
        Assert.Equal(scalableWidths1, scalableWidths2);
    }
}
