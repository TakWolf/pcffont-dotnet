namespace PcfSpec;

public enum PcfTableType : uint
{
    Properties = 1 << 0,
    Accelerators = 1 << 1,
    Metrics = 1 << 2,
    Bitmaps = 1 << 3,
    InkMetrics = 1 << 4,
    BdfEncodings = 1 << 5,
    ScalableWidths = 1 << 6,
    GlyphNames = 1 << 7,
    BdfAccelerators = 1 << 8
}
