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

    public static PcfTableFormat Of(
        bool msByteFirst = false,
        bool msBitFirst = false,
        bool inkBoundsOrCompressedMetrics = false,
        uint glyphPad = 1,
        uint scanUnit = 1)
    {
        var value = Default.Value;
        if (msByteFirst)
        {
            value |= FlagMsByteFirst;
        }
        if (msBitFirst)
        {
            value |= FlagMsBitFirst;
        }
        if (inkBoundsOrCompressedMetrics)
        {
            value |= FlagInkBoundsOrCompressedMetrics;
        }
        value |= (uint)GlyphPadToIndex(glyphPad);
        value |= (uint)ScanUnitToIndex(scanUnit) << 4;
        return new PcfTableFormat(value);
    }

    public uint Value { get; }

    public PcfTableFormat(uint value) => Value = value;

    public bool MsByteFirst => (Value & FlagMsByteFirst) != 0;

    public PcfTableFormat WithMsByteFirst(bool enabled) => new(enabled ? Value | FlagMsByteFirst : Value & ~FlagMsByteFirst);

    public bool MsBitFirst => (Value & FlagMsBitFirst) != 0;

    public PcfTableFormat WithMsBitFirst(bool enabled) => new(enabled ? Value | FlagMsBitFirst : Value & ~FlagMsBitFirst);

    public bool InkBounds => (Value & FlagInkBoundsOrCompressedMetrics) != 0;

    public PcfTableFormat WithInkBounds(bool enabled) => new(enabled ? Value | FlagInkBoundsOrCompressedMetrics : Value & ~FlagInkBoundsOrCompressedMetrics);

    public bool CompressedMetrics => InkBounds;

    public PcfTableFormat WithCompressedMetrics(bool enabled) => WithInkBounds(enabled);

    public int GlyphPadIndex => (int)(Value & MaskGlyphPad);

    public PcfTableFormat WithGlyphPadIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, GlyphPadOptions.Length);
        return new PcfTableFormat((Value & ~MaskGlyphPad) | (uint)index);
    }

    public uint GlyphPad => GlyphPadOptions[GlyphPadIndex];

    public PcfTableFormat WithGlyphPad(uint glyphPad) => WithGlyphPadIndex(GlyphPadToIndex(glyphPad));

    public int ScanUnitIndex => (int)((Value & MaskScanUnit) >> 4);

    public PcfTableFormat WithScanUnitIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ScanUnitOptions.Length);
        return new PcfTableFormat((Value & ~MaskScanUnit) | ((uint)index << 4));
    }

    public uint ScanUnit => ScanUnitOptions[ScanUnitIndex];

    public PcfTableFormat WithScanUnit(uint scanUnit) => WithScanUnitIndex(ScanUnitToIndex(scanUnit));

    public static implicit operator PcfTableFormat(uint value) => new(value);

    public static implicit operator uint(PcfTableFormat value) => value.Value;

    public bool Equals(PcfTableFormat other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is PcfTableFormat other && Equals(other);

    public static bool operator ==(PcfTableFormat left, PcfTableFormat right) => left.Equals(right);

    public static bool operator !=(PcfTableFormat left, PcfTableFormat right) => !(left == right);

    public override int GetHashCode() => Value.GetHashCode();
}
