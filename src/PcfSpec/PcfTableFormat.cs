namespace PcfSpec;

public readonly struct PcfTableFormat
{
    private const uint DefaultValue = 0b_0000_0000_0000;
    private const uint FlagInkBoundsOrCompressedMetrics = 0b_0001_0000_0000;

    private const uint MaskGlyphPad = 0b_00_00_11;
    private const uint MaskByteOrder = 0b_00_01_00;
    private const uint MaskBitOrder = 0b_00_10_00;
    private const uint MaskScanUnit = 0b_11_00_00;

    public static PcfTableFormat Parse(uint value)
    {
        var isMsByteFirst = (value & MaskByteOrder) > 0;
        var isMsBitFirst = (value & MaskBitOrder) > 0;
        var isInkBoundsOrCompressedMetrics = (value & FlagInkBoundsOrCompressedMetrics) > 0;
        var glyphPadIndex = Convert.ToInt32(value & MaskGlyphPad);
        var scanUnitIndex = Convert.ToInt32((value & MaskScanUnit) >> 4);
        return new PcfTableFormat(
            isMsByteFirst,
            isMsBitFirst,
            isInkBoundsOrCompressedMetrics,
            glyphPadIndex,
            scanUnitIndex);
    }
    
    public readonly bool IsMsByteFirst;
    public readonly bool IsMsBitFirst;
    public readonly bool IsInkBoundsOrCompressedMetrics;
    public readonly int GlyphPadIndex;
    public readonly int ScanUnitIndex;

    public PcfTableFormat()
    {
        IsMsByteFirst = false;
        IsMsBitFirst = false;
        IsInkBoundsOrCompressedMetrics = false;
        GlyphPadIndex = 0;
        ScanUnitIndex = 0;
    }
    
    public PcfTableFormat(
        bool isMsByteFirst = false,
        bool isMsBitFirst = false,
        bool isInkBoundsOrCompressedMetrics = false,
        int glyphPadIndex = 0,
        int scanUnitIndex = 0)
    {
        IsMsByteFirst = isMsByteFirst;
        IsMsBitFirst = isMsBitFirst;
        IsInkBoundsOrCompressedMetrics = isInkBoundsOrCompressedMetrics;
        GlyphPadIndex = glyphPadIndex;
        ScanUnitIndex = scanUnitIndex;
    }
    
    public uint Value
    {
        get
        {
            var value = DefaultValue;
            if (IsMsByteFirst)
            {
                value |= MaskByteOrder;
            }
            if (IsMsBitFirst)
            {
                value |= MaskBitOrder;
            }
            if (IsInkBoundsOrCompressedMetrics)
            {
                value |= FlagInkBoundsOrCompressedMetrics;
            }
            value |= Convert.ToUInt32(GlyphPadIndex);
            value |= Convert.ToUInt32(ScanUnitIndex) << 4;
            return value;
        }
    }
}
