using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfAcceleratorsTests
{
    [Fact]
    public void TestNoBounds()
    {
        var accelerators = new PcfAccelerators(
            noOverlap: true,
            constantMetrics: true,
            terminalFont: true,
            constantWidth: true,
            inkInside: true);
        accelerators.CalculateBounds();
        Assert.True(accelerators.NoOverlap);
        Assert.True(accelerators.ConstantMetrics);
        Assert.True(accelerators.TerminalFont);
        Assert.True(accelerators.ConstantWidth);
        Assert.True(accelerators.InkInside);
    }

    [Fact]
    public void TestNoOverlapFalse()
    {
        var accelerators = new PcfAccelerators(
            maxOverlap: 5,
            minBounds: new PcfMetric(leftSideBearing: -2),
            maxBounds: new PcfMetric());
        accelerators.CalculateBounds();
        Assert.False(accelerators.NoOverlap);
    }

    [Fact]
    public void TestNoOverlapTrue()
    {
        var accelerators = new PcfAccelerators(
            maxOverlap: -1,
            minBounds: new PcfMetric(),
            maxBounds: new PcfMetric());
        accelerators.CalculateBounds();
        Assert.True(accelerators.NoOverlap);
    }

    [Fact]
    public void TestConstantMetricsTrueTerminalFontFalse()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 12,
            fontDescent: 4,
            minBounds: new PcfMetric(
                leftSideBearing: 1,
                rightSideBearing: 5,
                characterWidth: 6,
                ascent: 10,
                descent: 3),
            maxBounds: new PcfMetric(
                leftSideBearing: 1,
                rightSideBearing: 5,
                characterWidth: 6,
                ascent: 10,
                descent: 3));
        accelerators.CalculateBounds();
        Assert.True(accelerators.ConstantMetrics);
        Assert.False(accelerators.TerminalFont);
        Assert.True(accelerators.ConstantWidth);
    }

    [Fact]
    public void TestTerminalFontTrue()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 8,
            fontDescent: 2,
            minBounds: new PcfMetric(
                rightSideBearing: 10,
                characterWidth: 10,
                ascent: 8,
                descent: 2),
            maxBounds: new PcfMetric(
                rightSideBearing: 10,
                characterWidth: 10,
                ascent: 8,
                descent: 2));
        accelerators.CalculateBounds();
        Assert.True(accelerators.ConstantMetrics);
        Assert.True(accelerators.TerminalFont);
        Assert.True(accelerators.ConstantWidth);
    }

    [Fact]
    public void TestConstantWidthFalse()
    {
        var accelerators = new PcfAccelerators(
            minBounds: new PcfMetric(characterWidth: 5),
            maxBounds: new PcfMetric(characterWidth: 7));
        accelerators.CalculateBounds();
        Assert.False(accelerators.ConstantWidth);
    }

    [Fact]
    public void TestConstantWidthTrue()
    {
        var accelerators = new PcfAccelerators(
            minBounds: new PcfMetric(characterWidth: 5),
            maxBounds: new PcfMetric(characterWidth: 5));
        accelerators.CalculateBounds();
        Assert.True(accelerators.ConstantWidth);
    }

    [Fact]
    public void TestInkInsideAllConditionsMet()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 12,
            fontDescent: 5,
            minBounds: new PcfMetric(
                ascent: 10,
                descent: 3),
            maxBounds: new PcfMetric(
                ascent: 12,
                descent: 5));
        accelerators.CalculateBounds();
        Assert.True(accelerators.InkInside);
    }

    [Fact]
    public void TestInkInsideFalseMaxOverlapPositive()
    {
        var accelerators = new PcfAccelerators(
            maxOverlap: 1,
            minBounds: new PcfMetric(),
            maxBounds: new PcfMetric());
        accelerators.CalculateBounds();
        Assert.False(accelerators.InkInside);
    }

    [Fact]
    public void TestInkInsideFalseAscentExceeds()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 10,
            fontDescent: 5,
            minBounds: new PcfMetric(ascent: 12),
            maxBounds: new PcfMetric(ascent: 12));
        accelerators.CalculateBounds();
        Assert.False(accelerators.InkInside);
    }

    [Fact]
    public void TestInkInsideFalseDescentExceeds()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 10,
            fontDescent: 5,
            minBounds: new PcfMetric(
                ascent: 8,
                descent: 4),
            maxBounds: new PcfMetric(
                ascent: 10,
                descent: 6));
        accelerators.CalculateBounds();
        Assert.False(accelerators.InkInside);
    }

    [Fact]
    public void TestInkInsideFalseNegativeAscentBelowNegativeFontDescent()
    {
        var accelerators = new PcfAccelerators(
            fontAscent: 10,
            fontDescent: 5,
            minBounds: new PcfMetric(ascent: -6),
            maxBounds: new PcfMetric());
        accelerators.CalculateBounds();
        Assert.False(accelerators.InkInside);
    }

    [Fact]
    public void TestCalculateBoundsResetsWhenDifferent()
    {
        var accelerators = new PcfAccelerators(
            constantMetrics: true,
            terminalFont: true,
            minBounds: new PcfMetric(leftSideBearing: 1),
            maxBounds: new PcfMetric(leftSideBearing: 2));
        accelerators.CalculateBounds();
        Assert.False(accelerators.ConstantMetrics);
        Assert.False(accelerators.TerminalFont);
    }

    [Fact]
    public void TestCopy()
    {
        var accelerators1 = new PcfAccelerators(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            noOverlap: true,
            constantMetrics: true,
            terminalFont: true,
            constantWidth: true,
            inkInside: true,
            inkMetrics: true,
            drawRightToLeft: true,
            fontAscent: 1,
            fontDescent: 2,
            maxOverlap: 4,
            minBounds: new PcfMetric(1, 2, 3, 4, 5, 6),
            maxBounds: new PcfMetric(6, 5, 4, 3, 2, 1),
            inkMinBounds: new PcfMetric(7, 8, 9, 10, 11, 12),
            inkMaxBounds: new PcfMetric(12, 11, 10, 9, 8, 7)
        );

        var accelerators2 = accelerators1.Copy();
        Assert.Equal(accelerators1, accelerators2);
        Assert.NotSame(accelerators1, accelerators2);
        Assert.NotSame(accelerators1.TableFormat, accelerators2.TableFormat);
        Assert.NotSame(accelerators1.MinBounds, accelerators2.MinBounds);
        Assert.NotSame(accelerators1.MaxBounds, accelerators2.MaxBounds);
        Assert.NotSame(accelerators1.InkMinBounds, accelerators2.InkMinBounds);
        Assert.NotSame(accelerators1.InkMaxBounds, accelerators2.InkMaxBounds);
    }
}
