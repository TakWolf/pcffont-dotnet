using PcfSpec.Tables;
using PcfSpec.Utils;

namespace PcfSpec;

public class PcfFontConfig : ICopyable<PcfFontConfig>, IEquatable<PcfFontConfig>
{
    public int FontAscent { get; set; }
    public int FontDescent { get; set; }
    public ushort DefaultChar { get; set; }
    public bool DrawRightToLeft { get; set; }
    public bool MsByteFirst { get; set; }
    public bool MsBitFirst { get; set; }
    public uint GlyphPad { get; set; }
    public uint ScanUnit { get; set; }

    public PcfFontConfig(
        int fontAscent = 0,
        int fontDescent = 0,
        ushort defaultChar = PcfBdfEncodings.NoEncoding,
        bool drawRightToLeft = false,
        bool msByteFirst = false,
        bool msBitFirst = false,
        uint glyphPad = 1,
        uint scanUnit = 1)
    {
        FontAscent = fontAscent;
        FontDescent = fontDescent;
        DefaultChar = defaultChar;
        DrawRightToLeft = drawRightToLeft;
        MsByteFirst = msByteFirst;
        MsBitFirst = msBitFirst;
        GlyphPad = glyphPad;
        ScanUnit = scanUnit;
    }

    public PcfTableFormat ToTableFormat() => PcfTableFormat.Create(
        msByteFirst: MsByteFirst,
        msBitFirst: MsBitFirst,
        glyphPad: GlyphPad,
        scanUnit: ScanUnit);

    public PcfFontConfig Copy() => new(
        FontAscent,
        FontDescent,
        DefaultChar,
        DrawRightToLeft,
        MsByteFirst,
        MsBitFirst,
        GlyphPad,
        ScanUnit);

    public PcfFontConfig DeepCopy() => Copy();

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
               GlyphPad == other.GlyphPad &&
               ScanUnit == other.ScanUnit;
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
