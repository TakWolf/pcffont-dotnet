using PcfSpec.Internal;

namespace PcfSpec;

public class PcfMetric
{
    public static PcfMetric Parse(Stream stream, bool msByteFirst, bool compressed)
    {
        short leftSideBearing;
        short rightSideBearing;
        short characterWidth;
        short ascent;
        short descent;
        ushort attributes;
        if (compressed)
        {
            leftSideBearing = (short)(stream.ReadUInt8() - 0x80);
            rightSideBearing = (short)(stream.ReadUInt8() - 0x80);
            characterWidth = (short)(stream.ReadUInt8() - 0x80);
            ascent = (short)(stream.ReadUInt8() - 0x80);
            descent = (short)(stream.ReadUInt8() - 0x80);
            attributes = 0;
        }
        else
        {
            leftSideBearing = stream.ReadInt16(msByteFirst);
            rightSideBearing = stream.ReadInt16(msByteFirst);
            characterWidth = stream.ReadInt16(msByteFirst);
            ascent = stream.ReadInt16(msByteFirst);
            descent = stream.ReadInt16(msByteFirst);
            attributes = stream.ReadUInt16(msByteFirst);
        }
        return new PcfMetric(
            leftSideBearing,
            rightSideBearing,
            characterWidth,
            ascent,
            descent,
            attributes);
    }

    public short LeftSideBearing;
    public short RightSideBearing;
    public short CharacterWidth;
    public short Ascent;
    public short Descent;
    public ushort Attributes;

    public PcfMetric(
        short leftSideBearing,
        short rightSideBearing,
        short characterWidth,
        short ascent,
        short descent,
        ushort attributes = 0)
    {
        LeftSideBearing = leftSideBearing;
        RightSideBearing = rightSideBearing;
        CharacterWidth = characterWidth;
        Ascent = ascent;
        Descent = descent;
        Attributes = attributes;
    }

    public int Width => RightSideBearing - LeftSideBearing;

    public int Height => Ascent + Descent;

    public (int, int) Dimensions => (Width, Height);

    public int OffsetX => LeftSideBearing;

    public int OffsetY => -Descent;

    public (int, int) Offset => (OffsetX, OffsetY);

    public bool Compressible =>
        LeftSideBearing is >= -128 and <= 127 &&
        RightSideBearing is >= -128 and <= 127 &&
        CharacterWidth is >= -128 and <= 127 &&
        Ascent is >= -128 and <= 127 &&
        Descent is >= -128 and <= 127;

    public void Dump(Stream stream, bool msByteFirst, bool compressed)
    {
        if (compressed)
        {
            stream.WriteUInt8((byte)(LeftSideBearing + 0x80));
            stream.WriteUInt8((byte)(RightSideBearing + 0x80));
            stream.WriteUInt8((byte)(CharacterWidth + 0x80));
            stream.WriteUInt8((byte)(Ascent + 0x80));
            stream.WriteUInt8((byte)(Descent + 0x80));
        }
        else
        {
            stream.WriteInt16(LeftSideBearing, msByteFirst);
            stream.WriteInt16(RightSideBearing, msByteFirst);
            stream.WriteInt16(CharacterWidth, msByteFirst);
            stream.WriteInt16(Ascent, msByteFirst);
            stream.WriteInt16(Descent, msByteFirst);
            stream.WriteUInt16(Attributes, msByteFirst);
        }
    }

    public static bool Equals(PcfMetric? objA, PcfMetric? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA is null || objB is null)
        {
            return false;
        }
        return objA.LeftSideBearing == objB.LeftSideBearing &&
               objA.RightSideBearing == objB.RightSideBearing &&
               objA.CharacterWidth == objB.CharacterWidth &&
               objA.Ascent == objB.Ascent &&
               objA.Descent == objB.Descent &&
               objA.Attributes == objB.Attributes;
    }
}
