namespace PcfSpec.Table;

public class PcfAccelerators : IPcfTable
{
    public static PcfAccelerators Parse(Stream stream, PcfFont font, PcfHeader header)
    {
        // TODO
        return new PcfAccelerators();
    }

    public PcfTableFormat TableFormat { get; set; }

    public PcfAccelerators(PcfTableFormat? tableFormat = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
    }

    public uint Dump(Stream stream, PcfFont font, uint tableOffset)
    {
        // TODO
        return 0;
    }
}
