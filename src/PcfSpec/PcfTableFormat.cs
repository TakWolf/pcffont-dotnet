namespace PcfSpec;

public class PcfTableFormat
{
    private const uint DefaultValue = 0b_0000_0000_0000;
    private const uint FlagInkBoundsOrCompressedMetrics = 0b_0001_0000_0000;

    private const uint MaskGlyphPad = 0b_00_00_11;
    private const uint MaskByteOrder = 0b_00_01_00;
    private const uint MaskBitOrder = 0b_00_10_00;
    private const uint MaskScanUnit = 0b_11_00_00;

    public static PcfTableFormat Parse(uint value)
    {
        var msByteFirst = (value & MaskByteOrder) > 0;
        var msBitFirst = (value & MaskBitOrder) > 0;
        var inkBoundsOrCompressedMetrics = (value & FlagInkBoundsOrCompressedMetrics) > 0;
        var glyphPadIndex = Convert.ToInt32(value & MaskGlyphPad);
        var scanUnitIndex = Convert.ToInt32((value & MaskScanUnit) >> 4);
        return new PcfTableFormat(
            msByteFirst,
            msBitFirst,
            inkBoundsOrCompressedMetrics,
            glyphPadIndex,
            scanUnitIndex);
    }

    public bool MsByteFirst;
    public bool MsBitFirst;
    public bool InkBoundsOrCompressedMetrics;
    public int GlyphPadIndex;
    public int ScanUnitIndex;

    public PcfTableFormat(
        bool msByteFirst = false,
        bool msBitFirst = false,
        bool inkBoundsOrCompressedMetrics = false,
        int glyphPadIndex = 0,
        int scanUnitIndex = 0)
    {
        MsByteFirst = msByteFirst;
        MsBitFirst = msBitFirst;
        InkBoundsOrCompressedMetrics = inkBoundsOrCompressedMetrics;
        GlyphPadIndex = glyphPadIndex;
        ScanUnitIndex = scanUnitIndex;
    }

    public uint Value
    {
        get
        {
            var value = DefaultValue;
            if (MsByteFirst)
            {
                value |= MaskByteOrder;
            }
            if (MsBitFirst)
            {
                value |= MaskBitOrder;
            }
            if (InkBoundsOrCompressedMetrics)
            {
                value |= FlagInkBoundsOrCompressedMetrics;
            }
            value |= Convert.ToUInt32(GlyphPadIndex);
            value |= Convert.ToUInt32(ScanUnitIndex) << 4;
            return value;
        }
    }
}
