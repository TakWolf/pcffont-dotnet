namespace PcfSpec.Table;

public class PcfBitmaps : IPcfTable
{
    public static PcfBitmaps Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfBitmaps();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfBitmaps(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
