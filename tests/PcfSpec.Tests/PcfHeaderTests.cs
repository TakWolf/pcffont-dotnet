namespace PcfSpec.Tests;

public class PcfHeaderTests
{
    [Fact]
    public void TestCompareTo()
    {
        var header1 = new PcfHeader(
            tableType: PcfTableType.Accelerators,
            tableFormat: new PcfTableFormat(),
            tableSize: 0,
            tableOffset: 0);
        var header2 = new PcfHeader(
            tableType: PcfTableType.Bitmaps,
            tableFormat: new PcfTableFormat(),
            tableSize: 0,
            tableOffset: 0);
        Assert.Equal(-1, header1.CompareTo(header2));
        Assert.Equal(0, header1.CompareTo(header1));
        Assert.Equal(1, header2.CompareTo(header1));
        Assert.Equal(1, header1.CompareTo(null));
    }

    [Fact]
    public void TestCopy()
    {
        var header1 = new PcfHeader(
            tableType: PcfTableType.Accelerators,
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            tableSize: 10,
            tableOffset: 20);
        var header2 = header1.Copy();

        Assert.Equal(header1, header2);
        Assert.NotSame(header1, header2);
        Assert.Same(header1.TableFormat, header2.TableFormat);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var header1 = new PcfHeader(
            tableType: PcfTableType.Accelerators,
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            tableSize: 10,
            tableOffset: 20);
        var header2 = header1.DeepCopy();

        Assert.Equal(header1, header2);
        Assert.NotSame(header1, header2);
        Assert.NotSame(header1.TableFormat, header2.TableFormat);
    }

    [Fact]
    public void TestEquals()
    {
        var header1 = new PcfHeader(
            tableType: PcfTableType.Accelerators,
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            tableSize: 10,
            tableOffset: 20);
        var header2 = new PcfHeader(
            tableType: PcfTableType.Accelerators,
            tableFormat: new PcfTableFormat(true, true, true, 1, 2),
            tableSize: 10,
            tableOffset: 20);
        Assert.Equal(header1, header2);
    }
}
