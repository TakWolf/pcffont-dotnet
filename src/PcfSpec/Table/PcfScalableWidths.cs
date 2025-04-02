namespace PcfSpec.Table;

public class PcfScalableWidths : IPcfTable
{
    public static PcfScalableWidths Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfScalableWidths();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfScalableWidths(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
