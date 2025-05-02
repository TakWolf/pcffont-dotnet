using PcfSpec.Util;

namespace PcfSpec.Table;

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
        if (tableFormat.InkBoundsOrCompressedMetrics)
        {
            inkMinBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);
            inkMaxBounds = PcfMetric.Parse(stream, tableFormat.MsByteFirst, false);
        }

        var table = new PcfAccelerators(
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

        // Compat
        if (header.TableSize > stream.Position - header.TableOffset)
        {
            stream.Seek(header.TableOffset, SeekOrigin.Begin);
            var rawChunk = stream.ReadBytes(header.TableSize, throwOnEndOfStream: false);
            table.CompatInfo = (rawChunk, header.TableSize);
        }

        return table;
    }

    public PcfTableFormat TableFormat { get; set; }
    public bool NoOverlap;
    public bool ConstantMetrics;
    public bool TerminalFont;
    public bool ConstantWidth;
    public bool InkInside;
    public bool InkMetrics;
    public bool DrawRightToLeft;
    public int FontAscent;
    public int FontDescent;
    public int MaxOverlap;
    public PcfMetric? MinBounds;
    public PcfMetric? MaxBounds;
    public PcfMetric? InkMinBounds;
    public PcfMetric? InkMaxBounds;
    public (byte[], uint)? CompatInfo;

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

        if (TableFormat.InkBoundsOrCompressedMetrics)
        {
            InkMinBounds!.Dump(stream, TableFormat.MsByteFirst, false);
            InkMaxBounds!.Dump(stream, TableFormat.MsByteFirst, false);
        }

        long tableSize;

        // Compat
        if (CompatInfo is not null)
        {
            (var rawChunk, tableSize) = CompatInfo.Value;
            stream.WriteBytes(rawChunk.AsSpan((int)(stream.Position - tableOffset)));
        }
        else
        {
            stream.AlignTo4ByteWithNulls();
            tableSize = stream.Position - tableOffset;
        }

        return (uint)tableSize;
    }

    private static bool CompatInfoEquals((byte[], uint)? objA, (byte[], uint)? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        var (rawChunkA, tableSizeA) = objA.Value;
        var (rawChunkB, tableSizeB) = objB.Value;
        return rawChunkA.SequenceEqual(rawChunkB) &&
               tableSizeA == tableSizeB;
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
               PcfMetric.Equals(objA.InkMaxBounds, objB.InkMaxBounds) &&
               CompatInfoEquals(objA.CompatInfo, objB.CompatInfo);
    }
}
