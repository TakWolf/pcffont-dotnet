using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfBitmapsTests
{
    [Fact]
    public void TestCopy()
    {
        var bitmaps1 = new PcfBitmaps(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            bitmaps: [
                [[1, 0, 0, 1]],
                [[0, 1, 1, 0]]
            ]);
        var bitmaps2 = bitmaps1.Copy();

        Assert.Equal(bitmaps1, bitmaps2);
        Assert.NotSame(bitmaps1, bitmaps2);
        Assert.Same(bitmaps1.TableFormat, bitmaps2.TableFormat);

        foreach (var (bitmap1, bitmap2) in bitmaps1.Zip(bitmaps2))
        {
            Assert.Same(bitmap1, bitmap2);
        }
    }

    [Fact]
    public void TestDeepCopy()
    {
        var bitmaps1 = new PcfBitmaps(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            bitmaps: [
                [[1, 0, 0, 1]],
                [[0, 1, 1, 0]]
            ]);
        var bitmaps2 = bitmaps1.DeepCopy();

        Assert.Equal(bitmaps1, bitmaps2);
        Assert.NotSame(bitmaps1, bitmaps2);
        Assert.NotSame(bitmaps1.TableFormat, bitmaps2.TableFormat);

        foreach (var (bitmap1, bitmap2) in bitmaps1.Zip(bitmaps2))
        {
            Assert.NotSame(bitmap1, bitmap2);
            foreach (var (bitmapRow1, bitmapRow2) in bitmap1.Zip(bitmap2))
            {
                Assert.NotSame(bitmapRow1, bitmapRow2);
            }
        }
    }

    [Fact]
    public void TestEquals()
    {
        var bitmaps1 = new PcfBitmaps(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            bitmaps: [
                [[1, 0, 0, 1]],
                [[0, 1, 1, 0]]
            ]);
        var bitmaps2 = new PcfBitmaps(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            bitmaps: [
                [[1, 0, 0, 1]],
                [[0, 1, 1, 0]]
            ]);
        Assert.Equal(bitmaps1, bitmaps2);
    }
}
