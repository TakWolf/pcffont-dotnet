using PcfSpec.Utils;

namespace PcfSpec.Tables;

public class PcfAccelerators : IPcfTable, ICopyable<PcfAccelerators>, IEquatable<PcfAccelerators>
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

        if (MinBounds.Equals(MaxBounds))
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

    public PcfAccelerators Copy() => new(
        TableFormat,
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
        MinBounds,
        MaxBounds,
        InkMinBounds,
        InkMaxBounds);

    public PcfAccelerators DeepCopy() => new(
        TableFormat.DeepCopy(),
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
        MinBounds?.DeepCopy(),
        MaxBounds?.DeepCopy(),
        InkMinBounds?.DeepCopy(),
        InkMaxBounds?.DeepCopy());

    public bool Equals(PcfAccelerators? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return TableFormat.Equals(other.TableFormat) &&
               NoOverlap == other.NoOverlap &&
               ConstantMetrics == other.ConstantMetrics &&
               TerminalFont == other.TerminalFont &&
               ConstantWidth == other.ConstantWidth &&
               InkInside == other.InkInside &&
               InkMetrics == other.InkMetrics &&
               DrawRightToLeft == other.DrawRightToLeft &&
               FontAscent == other.FontAscent &&
               FontDescent == other.FontDescent &&
               MaxOverlap == other.MaxOverlap &&
               EqualityComparer<PcfMetric>.Default.Equals(MinBounds, other.MinBounds) &&
               EqualityComparer<PcfMetric>.Default.Equals(MaxBounds, other.MaxBounds) &&
               EqualityComparer<PcfMetric>.Default.Equals(InkMinBounds, other.InkMinBounds) &&
               EqualityComparer<PcfMetric>.Default.Equals(InkMaxBounds, other.InkMaxBounds);
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
        return Equals((PcfAccelerators)other);
    }

    public override int GetHashCode() => 0;
}
