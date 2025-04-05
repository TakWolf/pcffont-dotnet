using PcfSpec.Table;

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
        ushort defaultChar = PcfBdfEncodings.NoGlyphIndex,
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

    public PcfTableFormat ToTableFormat() => new(
        msByteFirst: MsByteFirst,
        msBitFirst: MsBitFirst,
        glyphPadIndex: GlyphPadIndex,
        scanUnitIndex: ScanUnitIndex);
}
