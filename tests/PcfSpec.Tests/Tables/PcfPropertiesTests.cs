using PcfSpec.Errors;
using PcfSpec.Tables;

namespace PcfSpec.Tests.Tables;

public class PcfPropertiesTests
{
    [Fact]
    public void TestProperties1()
    {
        var properties = new PcfProperties(new Dictionary<string, PcfPropertyValue>
        {
            { "PARAM_1", 1 },
            { "param_2", "2" }
        });

        Assert.Equal(2, properties.Count);
        Assert.Equal(1, properties["param_1"]);
        Assert.Equal("2", properties["PARAM_2"]);
    }

    [Fact]
    public void TestProperties2()
    {
        var properties = new PcfProperties();

        properties.Foundry = "TakWolf Studio";
        Assert.Equal("TakWolf Studio", properties.Foundry);
        Assert.Equal("TakWolf Studio", properties["FOUNDRY"]);

        properties.FamilyName = "Demo Pixel";
        Assert.Equal("Demo Pixel", properties.FamilyName);
        Assert.Equal("Demo Pixel", properties["FAMILY_NAME"]);

        properties.WeightName = "Medium";
        Assert.Equal("Medium", properties.WeightName);
        Assert.Equal("Medium", properties["WEIGHT_NAME"]);

        properties.Slant = "R";
        Assert.Equal("R", properties.Slant);
        Assert.Equal("R", properties["SLANT"]);

        properties.SetWidthName = "Normal";
        Assert.Equal("Normal", properties.SetWidthName);
        Assert.Equal("Normal", properties["SETWIDTH_NAME"]);

        properties.AddStyleName = "Sans Serif";
        Assert.Equal("Sans Serif", properties.AddStyleName);
        Assert.Equal("Sans Serif", properties["ADD_STYLE_NAME"]);

        properties.PixelSize = 16;
        Assert.Equal(16, properties.PixelSize);
        Assert.Equal(16, properties["PIXEL_SIZE"]);

        properties.PointSize = 160;
        Assert.Equal(160, properties.PointSize);
        Assert.Equal(160, properties["POINT_SIZE"]);

        properties.ResolutionX = 75;
        Assert.Equal(75, properties.ResolutionX);
        Assert.Equal(75, properties["RESOLUTION_X"]);

        properties.ResolutionY = 240;
        Assert.Equal(240, properties.ResolutionY);
        Assert.Equal(240, properties["RESOLUTION_Y"]);

        properties.Spacing = "M";
        Assert.Equal("M", properties.Spacing);
        Assert.Equal("M", properties["SPACING"]);

        properties.AverageWidth = 85;
        Assert.Equal(85, properties.AverageWidth);
        Assert.Equal(85, properties["AVERAGE_WIDTH"]);

        properties.CharsetRegistry = "ISO8859";
        Assert.Equal("ISO8859", properties.CharsetRegistry);
        Assert.Equal("ISO8859", properties["CHARSET_REGISTRY"]);

        properties.CharsetEncoding = "1";
        Assert.Equal("1", properties.CharsetEncoding);
        Assert.Equal("1", properties["CHARSET_ENCODING"]);

        Assert.Equal(14, properties.Count);
        properties.GenerateXlfd();
        Assert.Equal("-TakWolf Studio-Demo Pixel-Medium-R-Normal-Sans Serif-16-160-75-240-M-85-ISO8859-1", properties.Font);
    }

    [Fact]
    public void TestProperties3()
    {
        var properties = new PcfProperties();

        properties.Font = "-Bitstream-Charter-Medium-R-Normal--12-120-75-75-P-68-ISO8859-1";
        properties.UpdateByXlfd();
        Assert.Equal("Bitstream", properties.Foundry);
        Assert.Equal("Charter", properties.FamilyName);
        Assert.Equal("Medium", properties.WeightName);
        Assert.Equal("R", properties.Slant);
        Assert.Equal("Normal", properties.SetWidthName);
        Assert.Null(properties.AddStyleName);
        Assert.Equal(12, properties.PixelSize);
        Assert.Equal(120, properties.PointSize);
        Assert.Equal(75, properties.ResolutionX);
        Assert.Equal(75, properties.ResolutionY);
        Assert.Equal("P", properties.Spacing);
        Assert.Equal(68, properties.AverageWidth);
        Assert.Equal("ISO8859", properties.CharsetRegistry);
        Assert.Equal("1", properties.CharsetEncoding);
    }

    [Fact]
    public void TestProperties4()
    {
        var properties = new PcfProperties();

        properties.Font = "--------------";
        properties.UpdateByXlfd();
        Assert.Null(properties.Foundry);
        Assert.Null(properties.FamilyName);
        Assert.Null(properties.WeightName);
        Assert.Null(properties.Slant);
        Assert.Null(properties.SetWidthName);
        Assert.Null(properties.AddStyleName);
        Assert.Null(properties.PixelSize);
        Assert.Null(properties.PointSize);
        Assert.Null(properties.ResolutionX);
        Assert.Null(properties.ResolutionY);
        Assert.Null(properties.Spacing);
        Assert.Null(properties.AverageWidth);
        Assert.Null(properties.CharsetRegistry);
        Assert.Null(properties.CharsetEncoding);
    }

    [Fact]
    public void TestProperties5()
    {
        var properties = new PcfProperties();

        properties.Font = "Bitstream-Charter-Medium-R-Normal--12-120-75-75-P-68-ISO8859-1";
        var e = Assert.Throws<PcfXlfdException>(() => properties.UpdateByXlfd());
        Assert.Equal("Must start with '-'.", e.Message);
    }

    [Fact]
    public void TestProperties6()
    {
        var properties = new PcfProperties();

        properties.Font = "-Bitstream-Charter-Medium-R-Normal--12-120-75-75-P-68-ISO8859-1-";
        var e = Assert.Throws<PcfXlfdException>(() => properties.UpdateByXlfd());
        Assert.Equal("Must contain 14 XLFD fields.", e.Message);
    }

    [Fact]
    public void TestProperties7()
    {
        var properties = new PcfProperties();

        properties.XHeight = 5;
        Assert.Equal(5, properties.XHeight);
        Assert.Equal(5, properties["X_HEIGHT"]);

        properties.CapHeight = 8;
        Assert.Equal(8, properties.CapHeight);
        Assert.Equal(8, properties["CAP_HEIGHT"]);

        properties.UnderlinePosition = -2;
        Assert.Equal(-2, properties.UnderlinePosition);
        Assert.Equal(-2, properties["UNDERLINE_POSITION"]);

        properties.UnderlineThickness = 1;
        Assert.Equal(1, properties.UnderlineThickness);
        Assert.Equal(1, properties["UNDERLINE_THICKNESS"]);

        Assert.Equal(4, properties.Count);
    }

    [Fact]
    public void TestProperties8()
    {
        var properties = new PcfProperties();

        properties.FontVersion = "1.0.0";
        Assert.Equal("1.0.0", properties.FontVersion);
        Assert.Equal("1.0.0", properties["FONT_VERSION"]);

        properties.Copyright = "Copyright (c) TakWolf";
        Assert.Equal("Copyright (c) TakWolf", properties.Copyright);
        Assert.Equal("Copyright (c) TakWolf", properties["COPYRIGHT"]);

        properties.Notice = "This is a notice.";
        Assert.Equal("This is a notice.", properties.Notice);
        Assert.Equal("This is a notice.", properties["NOTICE"]);

        Assert.Equal(3, properties.Count);
    }

    [Fact]
    public void TestProperties9()
    {
        var properties = new PcfProperties();

        properties["abc"] = "abc";
        Assert.Equal("abc", properties["ABC"]);
        Assert.Equal("abc", properties["abc"]);
    }

    [Fact]
    public void TestProperties10()
    {
        var properties = new PcfProperties();

        var e = Assert.Throws<PcfKeyException>(() => properties["abc-def"] = "abcdef");
        Assert.Equal("Key contain illegal characters.", e.Message);
    }

    [Fact]
    public void TestProperties11()
    {
        var properties = new PcfProperties();

        properties.SetValue("NULL_PARAM", null);
        Assert.Null(properties.GetValue("NULL_PARAM"));
    }

    [Fact]
    public void TestProperties12()
    {
        var properties = new PcfProperties();

        {
            var e = Assert.Throws<PcfValueException>(() => properties.SetValue("FOUNDRY", 1));
            Assert.Equal("Value of 'FOUNDRY' must be 'string'.", e.Message);
        }
        {
            var e = Assert.Throws<PcfValueException>(() => properties.SetValue("PIXEL_SIZE", "1"));
            Assert.Equal("Value of 'PIXEL_SIZE' must be 'int'.", e.Message);
        }
    }

    [Fact]
    public void TestCopy()
    {
        var properties1 = new PcfProperties(
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4));
        properties1.FamilyName = "Demo Font";
        properties1.PointSize = 100;
        var properties2 = properties1.Copy();
        var properties3 = properties1.DeepCopy();

        Assert.Equal(properties1, properties2);
        Assert.Equal(properties1, properties3);
        Assert.NotSame(properties1, properties2);
        Assert.NotSame(properties1, properties3);
    }

    [Fact]
    public void TestEquals()
    {
        var properties1 = new PcfProperties(
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4));
        properties1.FamilyName = "Demo Font";
        properties1.PointSize = 100;

        var properties2 = new PcfProperties(
            tableFormat: PcfTableFormat.Create(true, true, true, 2, 4));
        properties2.FamilyName = "Demo Font";
        properties2.PointSize = 100;

        Assert.Equal(properties1, properties2);
    }
}
