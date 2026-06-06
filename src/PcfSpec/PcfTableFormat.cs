using PcfSpec.Utils;

namespace PcfSpec;

public class PcfTableFormat : ICopyable<PcfTableFormat>, IEquatable<PcfTableFormat>
{
    private const uint DefaultValue = 0b_0000_0000_0000;
    private const uint FlagInkBoundsOrCompressedMetrics = 0b_0001_0000_0000;

    private const uint MaskGlyphPad = 0b_00_00_11;
    private const uint MaskByteOrder = 0b_00_01_00;
    private const uint MaskBitOrder = 0b_00_10_00;
    private const uint MaskScanUnit = 0b_11_00_00;

    public static readonly uint[] GlyphPadOptions = [1, 2, 4, 8];
    public static readonly uint[] ScanUnitOptions = [1, 2, 4];

    public static int GlyphPadToIndex(uint glyphPad)
    {
        var index = Array.IndexOf(GlyphPadOptions, glyphPad);
        if (index < 0)
        {
            throw new ArgumentException($"'{nameof(glyphPad)}' must be one of [{string.Join(", ", GlyphPadOptions)}].", nameof(glyphPad));
        }
        return index;
    }

    public static int ScanUnitToIndex(uint scanUnit)
    {
        var index = Array.IndexOf(ScanUnitOptions, scanUnit);
        if (index < 0)
        {
            throw new ArgumentException($"'{nameof(scanUnit)}' must be one of [{string.Join(", ", ScanUnitOptions)}].", nameof(scanUnit));
        }
        return index;
    }

    public static PcfTableFormat Parse(uint value)
    {
        var msByteFirst = (value & MaskByteOrder) > 0;
        var msBitFirst = (value & MaskBitOrder) > 0;
        var inkBoundsOrCompressedMetrics = (value & FlagInkBoundsOrCompressedMetrics) > 0;
        var glyphPadIndex = (int)(value & MaskGlyphPad);
        var scanUnitIndex = (int)((value & MaskScanUnit) >> 4);
        return new PcfTableFormat(
            msByteFirst,
            msBitFirst,
            inkBoundsOrCompressedMetrics,
            glyphPadIndex,
            scanUnitIndex);
    }

    public bool MsByteFirst { get; set; }
    public bool MsBitFirst { get; set; }
    public bool InkBoundsOrCompressedMetrics { get; set; }
    public int GlyphPadIndex { get; set; }
    public int ScanUnitIndex { get; set; }

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

    public bool InkBounds
    {
        get => InkBoundsOrCompressedMetrics;
        set => InkBoundsOrCompressedMetrics = value;
    }

    public bool CompressedMetrics
    {
        get => InkBoundsOrCompressedMetrics;
        set => InkBoundsOrCompressedMetrics = value;
    }

    public uint GlyphPad
    {
        get => GlyphPadOptions[GlyphPadIndex];
        set => GlyphPadIndex = GlyphPadToIndex(value);
    }

    public uint ScanUnit
    {
        get => ScanUnitOptions[ScanUnitIndex];
        set => ScanUnitIndex = ScanUnitToIndex(value);
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
            value |= (uint)GlyphPadIndex;
            value |= (uint)ScanUnitIndex << 4;
            return value;
        }
    }

    public PcfTableFormat Copy() => new(
        MsByteFirst,
        MsBitFirst,
        InkBoundsOrCompressedMetrics,
        GlyphPadIndex,
        ScanUnitIndex);

    public PcfTableFormat DeepCopy() => Copy();

    public bool Equals(PcfTableFormat? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return MsByteFirst == other.MsByteFirst &&
               MsBitFirst == other.MsBitFirst &&
               InkBoundsOrCompressedMetrics == other.InkBoundsOrCompressedMetrics &&
               GlyphPadIndex == other.GlyphPadIndex &&
               ScanUnitIndex == other.ScanUnitIndex;
    }

    public override bool Equals(object? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        return Equals((PcfTableFormat)other);
    }

    public override int GetHashCode() => 0;
}
