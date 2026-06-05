using PcfSpec.Tables;

namespace PcfSpec;

public class PcfFontConfig
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
}
