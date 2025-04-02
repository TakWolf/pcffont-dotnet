namespace PcfSpec.Table;

public class PcfMetrics : IPcfTable
{
    public static PcfMetrics Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfMetrics();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfMetrics(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
