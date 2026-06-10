namespace PcfSpec.Tests;

public class PcfGlyphTests
{
    [Fact]
    public void TestGlyph()
    {
        var glyph = new PcfGlyph(
            name: "_",
            encoding: 0,
            dimensions: (1, 2),
            offset: (3, 4));
        Assert.Equal(1, glyph.Width);
        Assert.Equal(2, glyph.Height);
        Assert.Equal((1, 2), glyph.Dimensions);
        Assert.Equal(3, glyph.OffsetX);
        Assert.Equal(4, glyph.OffsetY);
        Assert.Equal((3, 4), glyph.Offset);
    }

    [Fact]
    public void TestCreateMetric1()
    {
        var glyph = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 5,
            dimensions: (5, 8),
            offset: (0, -2),
            bitmap: [
                [0, 0, 0, 0, 0],
                [0, 1, 1, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 1, 1, 0],
                [0, 0, 0, 0, 0]
            ],
            attributes: 1);
        Assert.Equal(new PcfMetric(
            leftSideBearing: 0,
            rightSideBearing: 5,
            characterWidth: 5,
            ascent: 6,
            descent: 2,
            attributes: 1), glyph.CreateMetric(false));
        Assert.Equal(new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 4,
            characterWidth: 5,
            ascent: 5,
            descent: 1,
            attributes: 1), glyph.CreateMetric(true));
    }

    [Fact]
    public void TestCreateMetric2()
    {
        var glyph = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 5,
            dimensions: (7, 10),
            offset: (0, -4),
            bitmap: [
                [0, 0, 0, 0, 0],
                [0, 1, 1, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 0, 1, 0],
                [0, 1, 1, 1, 0],
                [0, 0, 0, 0, 0]
            ],
            attributes: 1);
        Assert.Equal(new PcfMetric(
            leftSideBearing: 0,
            rightSideBearing: 7,
            characterWidth: 5,
            ascent: 6,
            descent: 4,
            attributes: 1), glyph.CreateMetric(false));
        Assert.Equal(new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 4,
            characterWidth: 5,
            ascent: 5,
            descent: 1,
            attributes: 1), glyph.CreateMetric(true));
    }

    [Fact]
    public void TestCopy()
    {
        var glyph1 = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 1,
            dimensions: (2, 3),
            offset: (4, 5),
            bitmap: [[1, 0, 0, 1]],
            attributes: 1);
        var glyph2 = glyph1.Copy();

        Assert.Equal(glyph1, glyph2);
        Assert.NotSame(glyph1, glyph2);
        Assert.Same(glyph1.Bitmap, glyph2.Bitmap);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var glyph1 = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 1,
            dimensions: (2, 3),
            offset: (4, 5),
            bitmap: [[1, 0, 0, 1]],
            attributes: 1);
        var glyph2 = glyph1.DeepCopy();

        Assert.Equal(glyph1, glyph2);
        Assert.NotSame(glyph1, glyph2);
        Assert.NotSame(glyph1.Bitmap, glyph2.Bitmap);

        foreach (var (bitmapRow1, bitmapRow2) in glyph1.Bitmap.Zip(glyph2.Bitmap))
        {
            Assert.NotSame(bitmapRow1, bitmapRow2);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var glyph1 = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 1,
            dimensions: (2, 3),
            offset: (4, 5),
            bitmap: [[1, 0, 0, 1]],
            attributes: 1);
        var glyph2 = new PcfGlyph(
            name: "_",
            encoding: 0,
            characterWidth: 1,
            dimensions: (2, 3),
            offset: (4, 5),
            bitmap: [[1, 0, 0, 1]],
            attributes: 1);
        Assert.Equal(glyph1, glyph2);
    }
}
