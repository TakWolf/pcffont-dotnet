using PcfSpec.Utils;

namespace PcfSpec.Tests;

public class PcfMetricTests
{
    [Fact]
    public void TestEquals()
    {
        var metric1 = new PcfMetric(
            leftSideBearing: -3,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5,
            attributes: 1);
        var metric2 = new PcfMetric(
            leftSideBearing: -3,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5,
            attributes: 1);
        var metric3 = new PcfMetric(
            leftSideBearing: -2,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5,
            attributes: 1);
        Assert.Equal(metric1, metric2);
        Assert.NotEqual(metric1, metric3);
    }

    [Fact]
    public void TestCompressible()
    {
        var metric = new PcfMetric(attributes: 1);
        Assert.False(metric.Compressible);
        metric.Attributes = 0;
        Assert.True(metric.Compressible);

        metric.LeftSideBearing = -129;
        Assert.False(metric.Compressible);
        metric.LeftSideBearing = -128;
        Assert.True(metric.Compressible);
        metric.LeftSideBearing = 128;
        Assert.False(metric.Compressible);
        metric.LeftSideBearing = 127;
        Assert.True(metric.Compressible);

        metric.RightSideBearing = -129;
        Assert.False(metric.Compressible);
        metric.RightSideBearing = -128;
        Assert.True(metric.Compressible);
        metric.RightSideBearing = 128;
        Assert.False(metric.Compressible);
        metric.RightSideBearing = 127;
        Assert.True(metric.Compressible);

        metric.CharacterWidth = -129;
        Assert.False(metric.Compressible);
        metric.CharacterWidth = -128;
        Assert.True(metric.Compressible);
        metric.CharacterWidth = 128;
        Assert.False(metric.Compressible);
        metric.CharacterWidth = 127;
        Assert.True(metric.Compressible);

        metric.Ascent = -129;
        Assert.False(metric.Compressible);
        metric.Ascent = -128;
        Assert.True(metric.Compressible);
        metric.Ascent = 128;
        Assert.False(metric.Compressible);
        metric.Ascent = 127;
        Assert.True(metric.Compressible);

        metric.Descent = -129;
        Assert.False(metric.Compressible);
        metric.Descent = -128;
        Assert.True(metric.Compressible);
        metric.Descent = 128;
        Assert.False(metric.Compressible);
        metric.Descent = 127;
        Assert.True(metric.Compressible);
    }

    [Fact]
    public void TestCopy()
    {
        var metric1 = new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 2,
            characterWidth: 3,
            ascent: 4,
            descent: 5,
            attributes: 6);

        var metric2 = metric1.Copy();
        Assert.Equal(metric1, metric2);
        Assert.NotSame(metric1, metric2);
    }

    [Fact]
    public void TestCreateByGlyph()
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
    public void TestCalculate1()
    {
        PcfMetric[] metrics = [
            new(
                leftSideBearing: -3,
                rightSideBearing: 8,
                characterWidth: 4,
                ascent: 9,
                descent: -5,
                attributes: 0b_00000001),
            new(
                leftSideBearing: 7,
                rightSideBearing: 3,
                characterWidth: 1,
                ascent: -6,
                descent: 0,
                attributes: 0b_00010001),
            new(
                leftSideBearing: 1,
                rightSideBearing: 0,
                characterWidth: 2,
                ascent: 5,
                descent: 4,
                attributes: 0b_10000001),
            new(
                leftSideBearing: -5,
                rightSideBearing: -1,
                characterWidth: 7,
                ascent: -3,
                descent: -9,
                attributes: 0b_01100001)
        ];
        Assert.Equal(4, CalculateUtil.CalculateMaxOverlap(metrics));
        Assert.Equal(new PcfMetric(
            leftSideBearing: -5,
            rightSideBearing: -1,
            characterWidth: 1,
            ascent: -6,
            descent: -9,
            attributes: 0b_00000001), CalculateUtil.CalculateMinBounds(metrics));
        Assert.Equal(new PcfMetric(
            leftSideBearing: 7,
            rightSideBearing: 8,
            characterWidth: 7,
            ascent: 9,
            descent: 4,
            attributes: 0b_11110001), CalculateUtil.CalculateMaxBounds(metrics));
    }

    [Fact]
    public void TestCalculate2()
    {
        var metrics = Array.Empty<PcfMetric>();
        Assert.Equal(0, CalculateUtil.CalculateMaxOverlap(metrics));
        Assert.Equal(new PcfMetric(), CalculateUtil.CalculateMinBounds(metrics));
        Assert.Equal(new PcfMetric(), CalculateUtil.CalculateMaxBounds(metrics));
    }
}
