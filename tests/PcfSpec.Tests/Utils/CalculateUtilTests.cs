using PcfSpec.Utils;

namespace PcfSpec.Tests.Utils;

public class CalculateUtilTests
{
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
