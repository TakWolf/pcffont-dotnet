namespace PcfSpec;

public readonly struct PcfTableFormat : IEquatable<PcfTableFormat>
{
    public static readonly uint[] GlyphPadOptions = [1, 2, 4, 8];
    public static readonly uint[] ScanUnitOptions = [1, 2, 4];

    private static int GlyphPadToIndex(uint glyphPad)
    {
        var index = Array.IndexOf(GlyphPadOptions, glyphPad);
        if (index < 0)
        {
            throw new ArgumentException($"'{nameof(glyphPad)}' must be one of [{string.Join(", ", GlyphPadOptions)}].", nameof(glyphPad));
        }
        return index;
    }

    private static int ScanUnitToIndex(uint scanUnit)
    {
        var index = Array.IndexOf(ScanUnitOptions, scanUnit);
        if (index < 0)
        {
            throw new ArgumentException($"'{nameof(scanUnit)}' must be one of [{string.Join(", ", ScanUnitOptions)}].", nameof(scanUnit));
        }
        return index;
    }

    private const uint FlagMsByteFirst = 0b_01_00;
    private const uint FlagMsBitFirst = 0b_10_00;
    private const uint FlagInkBoundsOrCompressedMetrics = 0b_01_0000_0000;

    private const uint MaskGlyphPad = 0b_00_00_11;
    private const uint MaskScanUnit = 0b_11_00_00;

    public static readonly PcfTableFormat Default = default;

    public static PcfTableFormat Create(
        bool msByteFirst = false,
        bool msBitFirst = false,
        bool inkBoundsOrCompressedMetrics = false,
        uint glyphPad = 1,
        uint scanUnit = 1) => Default.With(
        msByteFirst,
        msBitFirst,
        inkBoundsOrCompressedMetrics,
        glyphPad,
        scanUnit);

    public uint Value { get; }

    public PcfTableFormat(uint value) => Value = value;

    public bool MsByteFirst => (Value & FlagMsByteFirst) != 0;

    public bool MsBitFirst => (Value & FlagMsBitFirst) != 0;

    public bool InkBounds => (Value & FlagInkBoundsOrCompressedMetrics) != 0;

    public bool CompressedMetrics => InkBounds;

    public int GlyphPadIndex => (int)(Value & MaskGlyphPad);

    public uint GlyphPad => GlyphPadOptions[GlyphPadIndex];

    public int ScanUnitIndex => (int)((Value & MaskScanUnit) >> 4);

    public uint ScanUnit => ScanUnitOptions[ScanUnitIndex];

    public PcfTableFormat With(
        bool? msByteFirst = null,
        bool? msBitFirst = null,
        bool? inkBoundsOrCompressedMetrics = null,
        uint? glyphPad = null,
        uint? scanUnit = null)
    {
        var value = Value;
        if (msByteFirst is not null)
        {
            value = msByteFirst.Value ? value | FlagMsByteFirst : value & ~FlagMsByteFirst;
        }
        if (msBitFirst is not null)
        {
            value = msBitFirst.Value ? value | FlagMsBitFirst : value & ~FlagMsBitFirst;
        }
        if (inkBoundsOrCompressedMetrics is not null)
        {
            value = inkBoundsOrCompressedMetrics.Value ? value | FlagInkBoundsOrCompressedMetrics : value & ~FlagInkBoundsOrCompressedMetrics;
        }
        if (glyphPad is not null)
        {
            value = (value & ~MaskGlyphPad) | (uint)GlyphPadToIndex(glyphPad.Value);
        }
        if (scanUnit is not null)
        {
            value = (value & ~MaskScanUnit) | ((uint)ScanUnitToIndex(scanUnit.Value) << 4);
        }
        return new PcfTableFormat(value);
    }

    public static implicit operator PcfTableFormat(uint value) => new(value);

    public static implicit operator uint(PcfTableFormat value) => value.Value;

    public bool Equals(PcfTableFormat other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PcfTableFormat other && Equals(other);

    public static bool operator ==(PcfTableFormat left, PcfTableFormat right) => left.Equals(right);

    public static bool operator !=(PcfTableFormat left, PcfTableFormat right) => !(left == right);

    public override int GetHashCode() => Value.GetHashCode();
}
