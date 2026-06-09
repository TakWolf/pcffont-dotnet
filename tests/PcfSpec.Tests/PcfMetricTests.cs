namespace PcfSpec.Tests;

public class PcfMetricTests
{
    [Fact]
    public void TestMetric()
    {
        var metric = new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 2,
            characterWidth: 3,
            ascent: 4,
            descent: 5);
        Assert.Equal(1, metric.Width);
        Assert.Equal(9, metric.Height);
        Assert.Equal((1, 9), metric.Dimensions);
        Assert.Equal(1, metric.OffsetX);
        Assert.Equal(-5, metric.OffsetY);
        Assert.Equal((1, -5), metric.Offset);
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
        var metric3 = metric1.DeepCopy();

        Assert.Equal(metric1, metric2);
        Assert.Equal(metric1, metric3);
        Assert.NotSame(metric1, metric2);
        Assert.NotSame(metric1, metric3);
    }

    [Fact]
    public void TestEquals()
    {
        var metric1 = new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 2,
            characterWidth: 3,
            ascent: 4,
            descent: 5,
            attributes: 6);
        var metric2 = new PcfMetric(
            leftSideBearing: 1,
            rightSideBearing: 2,
            characterWidth: 3,
            ascent: 4,
            descent: 5,
            attributes: 6);
        Assert.Equal(metric1, metric2);
    }
}
