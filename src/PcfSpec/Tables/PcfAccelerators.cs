using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfAccelerators : IPcfTable
{
    public static PcfAccelerators Parse(Stream stream, PcfHeader header, PcfFont font)
    {
        var tableFormat = header.ReadAndCheckTableFormat(stream);

        var noOverlap = stream.ReadBool();
        var constantMetrics = stream.ReadBool();
        var terminalFont = stream.ReadBool();
        var constantWidth = stream.ReadBool();
        var inkInside = stream.ReadBool();
        var inkMetrics = stream.ReadBool();
        var drawRightToLeft = stream.ReadBool();
        stream.Seek(1, SeekOrigin.Current);
        var fontAscent = stream.ReadInt32(tableFormat.MsByteFirst);
        var fontDescent = stream.ReadInt32(tableFormat.MsByteFirst);
        var maxOverlap = stream.ReadInt32(tableFormat.MsByteFirst);

        var minBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);
        var maxBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);

        PcfMetric? inkMinBounds = null;
        PcfMetric? inkMaxBounds = null;
        if (tableFormat.InkBounds)
        {
            inkMinBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);
            inkMaxBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);
        }

        return new PcfAccelerators(
            tableFormat,
            noOverlap,
            constantMetrics,
            terminalFont,
            constantWidth,
            inkInside,
            inkMetrics,
            drawRightToLeft,
            fontAscent,
            fontDescent,
            maxOverlap,
            minBounds,
            maxBounds,
            inkMinBounds,
            inkMaxBounds);
    }

    public PcfTableFormat TableFormat { get; set; }
    public bool NoOverlap { get; set; }
    public bool ConstantMetrics { get; set; }
    public bool TerminalFont { get; set; }
    public bool ConstantWidth { get; set; }
    public bool InkInside { get; set; }
    public bool InkMetrics { get; set; }
    public bool DrawRightToLeft { get; set; }
    public int FontAscent { get; set; }
    public int FontDescent { get; set; }
    public int MaxOverlap { get; set; }
    public PcfMetric? MinBounds { get; set; }
    public PcfMetric? MaxBounds { get; set; }
    public PcfMetric? InkMinBounds { get; set; }
    public PcfMetric? InkMaxBounds { get; set; }

    public PcfAccelerators(
        PcfTableFormat? tableFormat = null,
        bool noOverlap = false,
        bool constantMetrics = false,
        bool terminalFont = false,
        bool constantWidth = false,
        bool inkInside = false,
        bool inkMetrics = false,
        bool drawRightToLeft = false,
        int fontAscent = 0,
        int fontDescent = 0,
        int maxOverlap = 0,
        PcfMetric? minBounds = null,
        PcfMetric? maxBounds = null,
        PcfMetric? inkMinBounds = null,
        PcfMetric? inkMaxBounds = null)
    {
        TableFormat = tableFormat ?? new PcfTableFormat();
        NoOverlap = noOverlap;
        ConstantMetrics = constantMetrics;
        TerminalFont = terminalFont;
        ConstantWidth = constantWidth;
        InkInside = inkInside;
        InkMetrics = inkMetrics;
        DrawRightToLeft = drawRightToLeft;
        FontAscent = fontAscent;
        FontDescent = fontDescent;
        MaxOverlap = maxOverlap;
        MinBounds = minBounds;
        MaxBounds = maxBounds;
        InkMinBounds = inkMinBounds;
        InkMaxBounds = inkMaxBounds;
    }

    public void CalculateBounds()
    {
        if (MinBounds is null || MaxBounds is null)
        {
            return;
        }

        NoOverlap = MaxOverlap <= MinBounds.LeftSideBearing;

        if (PcfMetric.Equals(MinBounds, MaxBounds))
        {
            ConstantMetrics = true;
            TerminalFont =
                MinBounds.LeftSideBearing == 0 &&
                MinBounds.RightSideBearing == MinBounds.CharacterWidth &&
                MinBounds.Ascent == FontAscent &&
                MinBounds.Descent == FontDescent;
        }
        else
        {
            ConstantMetrics = false;
            TerminalFont = false;
        }

        ConstantWidth = MinBounds.CharacterWidth == MaxBounds.CharacterWidth;
        InkInside =
            MaxOverlap <= 0 &&
            MinBounds.LeftSideBearing >= 0 &&
            MinBounds.Ascent >= -FontDescent &&
            MaxBounds.Ascent <= FontAscent &&
            -MinBounds.Descent <= FontAscent &&
            MaxBounds.Descent <= FontDescent;
    }

    public PcfAccelerators Copy() => new(
            TableFormat.Copy(),
            NoOverlap,
            ConstantMetrics,
            TerminalFont,
            ConstantWidth,
            InkInside,
            InkMetrics,
            DrawRightToLeft,
            FontAscent,
            FontDescent,
            MaxOverlap,
            MinBounds?.Copy(),
            MaxBounds?.Copy(),
            InkMinBounds?.Copy(),
            InkMaxBounds?.Copy());

    public uint Dump(Stream stream, uint tableOffset, PcfFont font)
    {
        stream.Seek(tableOffset, SeekOrigin.Begin);
        stream.WriteUInt32(TableFormat.Value);
        stream.WriteBool(NoOverlap);
        stream.WriteBool(ConstantMetrics);
        stream.WriteBool(TerminalFont);
        stream.WriteBool(ConstantWidth);
        stream.WriteBool(InkInside);
        stream.WriteBool(InkMetrics);
        stream.WriteBool(DrawRightToLeft);
        stream.WriteNulls(1);
        stream.WriteInt32(FontAscent, TableFormat.MsByteFirst);
        stream.WriteInt32(FontDescent, TableFormat.MsByteFirst);
        stream.WriteInt32(MaxOverlap, TableFormat.MsByteFirst);

        MinBounds!.Dump(stream, TableFormat.MsByteFirst, false);
        MaxBounds!.Dump(stream, TableFormat.MsByteFirst, false);

        if (TableFormat.InkBounds)
        {
            InkMinBounds!.Dump(stream, TableFormat.MsByteFirst, false);
            InkMaxBounds!.Dump(stream, TableFormat.MsByteFirst, false);
        }

        if (ReferenceEquals(this, font.Accelerators))
        {
            var tableSize = stream.Position - tableOffset;
            stream.WriteNulls((int)(100 - tableSize));
        }

        return 100;
    }

    public static bool Equals(PcfAccelerators? objA, PcfAccelerators? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        return objA.TableFormat.Value == objB.TableFormat.Value &&
               objA.NoOverlap == objB.NoOverlap &&
               objA.ConstantMetrics == objB.ConstantMetrics &&
               objA.TerminalFont == objB.TerminalFont &&
               objA.ConstantWidth == objB.ConstantWidth &&
               objA.InkInside == objB.InkInside &&
               objA.InkMetrics == objB.InkMetrics &&
               objA.DrawRightToLeft == objB.DrawRightToLeft &&
               objA.FontAscent == objB.FontAscent &&
               objA.FontDescent == objB.FontDescent &&
               objA.MaxOverlap == objB.MaxOverlap &&
               PcfMetric.Equals(objA.MinBounds, objB.MinBounds) &&
               PcfMetric.Equals(objA.MaxBounds, objB.MaxBounds) &&
               PcfMetric.Equals(objA.InkMinBounds, objB.InkMinBounds) &&
               PcfMetric.Equals(objA.InkMaxBounds, objB.InkMaxBounds);
    }
}
