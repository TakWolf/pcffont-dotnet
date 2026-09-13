using PcfSpec.Utils;

namespace PcfSpec.Tests.Utils;

public class CalculateUtilTests
{
    [Fact]
    public void TestCalculate()
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
    public void TestCalculateEmpty()
    {
        var metrics = Array.Empty<PcfMetric>();
        Assert.Equal(0, CalculateUtil.CalculateMaxOverlap(metrics));
        Assert.Equal(new PcfMetric(), CalculateUtil.CalculateMinBounds(metrics));
        Assert.Equal(new PcfMetric(), CalculateUtil.CalculateMaxBounds(metrics));
    }

    [Fact]
    public void TestCalculateBoundsIgnoreZeroGeometryRegardlessOfOrder()
    {
        var zero = new PcfMetric(attributes: 0b_0011);
        var metric1 = new PcfMetric(
            leftSideBearing: 5,
            rightSideBearing: 8,
            characterWidth: 6,
            ascent: 7,
            descent: 2,
            attributes: 0b_0101);
        var metric2 = new PcfMetric(
            leftSideBearing: -2,
            rightSideBearing: 4,
            characterWidth: 3,
            ascent: 9,
            descent: -1,
            attributes: 0b_1001);
        var expectedMinBounds = new PcfMetric(
            leftSideBearing: -2,
            rightSideBearing: 4,
            characterWidth: 3,
            ascent: 7,
            descent: -1,
            attributes: 0b_0001);
        var expectedMaxBounds = new PcfMetric(
            leftSideBearing: 5,
            rightSideBearing: 8,
            characterWidth: 6,
            ascent: 9,
            descent: 2,
            attributes: 0b_1111);
        PcfMetric[][] permutations = [
            [zero, metric1, metric2],
            [zero, metric2, metric1],
            [metric1, zero, metric2],
            [metric1, metric2, zero],
            [metric2, zero, metric1],
            [metric2, metric1, zero]
        ];
        foreach (var metrics in permutations)
        {
            Assert.Equal(expectedMinBounds, CalculateUtil.CalculateMinBounds(metrics));
            Assert.Equal(expectedMaxBounds, CalculateUtil.CalculateMaxBounds(metrics));
        }
    }

    [Fact]
    public void TestCalculateBoundsAllZeroGeometry()
    {
        PcfMetric[] metrics = [
            new(attributes: 0b_0011),
            new(attributes: 0b_0101)
        ];
        Assert.Equal(new PcfMetric(attributes: 0b_0001), CalculateUtil.CalculateMinBounds(metrics));
        Assert.Equal(new PcfMetric(attributes: 0b_0111), CalculateUtil.CalculateMaxBounds(metrics));
    }

    [Fact]
    public void TestCalculateBoundsIncludeNonzeroCharacterWidth()
    {
        PcfMetric[] metrics = [
            new(attributes: 0b_0011),
            new(characterWidth: 8, attributes: 0b_0101)
        ];
        Assert.Equal(new PcfMetric(characterWidth: 8, attributes: 0b_0001), CalculateUtil.CalculateMinBounds(metrics));
        Assert.Equal(new PcfMetric(characterWidth: 8, attributes: 0b_0111), CalculateUtil.CalculateMaxBounds(metrics));
    }

    [Fact]
    public void TestCalculateMaxOverlapIncludesZeroGeometry()
    {
        PcfMetric[] metrics = [
            new(rightSideBearing: 5, characterWidth: 8),
            new()
        ];
        Assert.Equal(0, CalculateUtil.CalculateMaxOverlap(metrics));
    }
}
