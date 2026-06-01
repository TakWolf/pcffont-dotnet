using PcfSpec.Tables;

namespace PcfSpec;

public class PcfFontConfig
{
    public int FontAscent;
    public int FontDescent;
    public ushort DefaultChar;
    public bool DrawRightToLeft;
    public bool MsByteFirst;
    public bool MsBitFirst;
    public int GlyphPadIndex;
    public int ScanUnitIndex;

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
        set
        {
            var index = Array.IndexOf(PcfTableFormat.GlyphPadOptions, value);
            if (index < 0)
            {
                throw new ArgumentException($"{nameof(GlyphPad)} must be one of [{string.Join(", ", PcfTableFormat.GlyphPadOptions)}].", nameof(GlyphPad));
            }
            GlyphPadIndex = index;
        }
    }

    public uint ScanUnit
    {
        get => PcfTableFormat.ScanUnitOptions[ScanUnitIndex];
        set
        {
            var index = Array.IndexOf(PcfTableFormat.ScanUnitOptions, value);
            if (index < 0)
            {
                throw new ArgumentException($"{nameof(ScanUnit)} must be one of [{string.Join(", ", PcfTableFormat.ScanUnitOptions)}].", nameof(ScanUnit));
            }
            ScanUnitIndex = index;
        }
    }

    public PcfTableFormat ToTableFormat() => new(
        msByteFirst: MsByteFirst,
        msBitFirst: MsBitFirst,
        glyphPadIndex: GlyphPadIndex,
        scanUnitIndex: ScanUnitIndex);
}
