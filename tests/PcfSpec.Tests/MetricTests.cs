namespace PcfSpec.Tests;

public class MetricTests
{
    [Fact]
    public void TestEquals()
    {
        var metric1 = new PcfMetric(
            leftSideBearing: -3,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5);
        var metric2 = new PcfMetric(
            leftSideBearing: -3,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5);
        var metric3 = new PcfMetric(
            leftSideBearing: -2,
            rightSideBearing: 8,
            characterWidth: 4,
            ascent: 9,
            descent: -5);
        Assert.True(PcfMetric.Equals(metric1, metric2));
        Assert.False(PcfMetric.Equals(metric1, metric3));
    }

    [Fact]
    public void TestCompressible()
    {
        var metric = new PcfMetric(
            leftSideBearing: 0,
            rightSideBearing: 0,
            characterWidth: 0,
            ascent: 0,
            descent: 0);
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
}
