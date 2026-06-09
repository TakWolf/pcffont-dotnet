using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfMetricsTests
{
    [Fact]
    public void TestCopy()
    {
        var metrics1 = new PcfMetrics(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            metrics: [
                new PcfMetric(1, 2, 3, 4, 5, 6),
                new PcfMetric(6, 5, 4, 3, 2, 1)
            ]);
        var metrics2 = metrics1.Copy();

        Assert.Equal(metrics1, metrics2);
        Assert.NotSame(metrics1, metrics2);
        Assert.Same(metrics1.TableFormat, metrics2.TableFormat);

        foreach (var (metric1, metric2) in metrics1.Zip(metrics2))
        {
            Assert.Same(metric1, metric2);
        }
    }

    [Fact]
    public void TestDeepCopy()
    {
        var metrics1 = new PcfMetrics(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            metrics: [
                new PcfMetric(1, 2, 3, 4, 5, 6),
                new PcfMetric(6, 5, 4, 3, 2, 1)
            ]);
        var metrics2 = metrics1.DeepCopy();

        Assert.Equal(metrics1, metrics2);
        Assert.NotSame(metrics1, metrics2);
        Assert.NotSame(metrics1.TableFormat, metrics2.TableFormat);

        foreach (var (metric1, metric2) in metrics1.Zip(metrics2))
        {
            Assert.NotSame(metric1, metric2);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var metrics1 = new PcfMetrics(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            metrics: [
                new PcfMetric(1, 2, 3, 4, 5, 6),
                new PcfMetric(6, 5, 4, 3, 2, 1)
            ]);
        var metrics2 = new PcfMetrics(
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            metrics: [
                new PcfMetric(1, 2, 3, 4, 5, 6),
                new PcfMetric(6, 5, 4, 3, 2, 1)
            ]);
        Assert.Equal(metrics1, metrics2);
    }
}
