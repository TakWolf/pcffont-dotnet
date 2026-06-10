using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfScalableWidthsTests
{
    [Fact]
    public void TestCopy()
    {
        var scalableWidths1 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var scalableWidths2 = scalableWidths1.Copy();

        Assert.Equal(scalableWidths1, scalableWidths2);
        Assert.NotSame(scalableWidths1, scalableWidths2);
        Assert.Same(scalableWidths1.TableFormat, scalableWidths2.TableFormat);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var scalableWidths1 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var scalableWidths2 = scalableWidths1.DeepCopy();

        Assert.Equal(scalableWidths1, scalableWidths2);
        Assert.NotSame(scalableWidths1, scalableWidths2);
        Assert.NotSame(scalableWidths1.TableFormat, scalableWidths2.TableFormat);
    }

    [Fact]
    public void TestEquals()
    {
        var scalableWidths1 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        var scalableWidths2 = new PcfScalableWidths(
            [1, 2, 3, 4],
            tableFormat: new PcfTableFormat(true, true, true, 1, 2));
        Assert.Equal(scalableWidths1, scalableWidths2);
    }
}
