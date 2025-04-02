namespace PcfSpec.Table;

public class PcfBdfEncodings : IPcfTable
{
    public static PcfBdfEncodings Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfBdfEncodings();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfBdfEncodings(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
