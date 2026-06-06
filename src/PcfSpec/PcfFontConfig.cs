using PcfSpec.Tables;

namespace PcfSpec;

public class PcfFontConfig : IEquatable<PcfFontConfig>
{
    public int FontAscent { get; set; }
    public int FontDescent { get; set; }
    public ushort DefaultChar { get; set; }
    public bool DrawRightToLeft { get; set; }
    public bool MsByteFirst { get; set; }
    public bool MsBitFirst { get; set; }
    public int GlyphPadIndex { get; set; }
    public int ScanUnitIndex { get; set; }

    public PcfFontConfig(
        int fontAscent = 0,
        int fontDescent = 0,
        ushort defaultChar = PcfBdfEncodings.NoEncoding,
        bool drawRightToLeft = false,
        bool msByteFirst = false,
        bool msBitFirst = false,
        int glyphPadIndex = 0,
        int scanUnitIndex = 0)
    {
        FontAscent = fontAscent;
        FontDescent = fontDescent;
        DefaultChar = defaultChar;
        DrawRightToLeft = drawRightToLeft;
        MsByteFirst = msByteFirst;
        MsBitFirst = msBitFirst;
        GlyphPadIndex = glyphPadIndex;
        ScanUnitIndex = scanUnitIndex;
    }

    public uint GlyphPad
    {
        get => PcfTableFormat.GlyphPadOptions[GlyphPadIndex];
        set => GlyphPadIndex = PcfTableFormat.GlyphPadToIndex(value);
    }

    public uint ScanUnit
    {
        get => PcfTableFormat.ScanUnitOptions[ScanUnitIndex];
        set => ScanUnitIndex = PcfTableFormat.ScanUnitToIndex(value);
    }

    public PcfTableFormat ToTableFormat() => new(
        msByteFirst: MsByteFirst,
        msBitFirst: MsBitFirst,
        glyphPadIndex: GlyphPadIndex,
        scanUnitIndex: ScanUnitIndex);

    public bool Equals(PcfFontConfig? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return FontAscent == other.FontAscent &&
               FontDescent == other.FontDescent &&
               DefaultChar == other.DefaultChar &&
               DrawRightToLeft == other.DrawRightToLeft &&
               MsByteFirst == other.MsByteFirst &&
               MsBitFirst == other.MsBitFirst &&
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
        return Equals((PcfFontConfig)other);
    }

    public override int GetHashCode() => 0;
}
