namespace PcfSpec.Table;

public class PcfProperties : IPcfTable
{
    public static PcfProperties Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfProperties();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfProperties(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
