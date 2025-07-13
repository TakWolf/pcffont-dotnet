# PcfFont.NET

[![.NET](https://img.shields.io/badge/dotnet-8.0-mediumpurple)](https://dotnet.microsoft.com)
[![NuGet](https://img.shields.io/nuget/v/PcfFont)](https://www.nuget.org/packages/PcfFont)

PcfFont is a library for manipulating [Portable Compiled Format (PCF) Fonts](https://en.wikipedia.org/wiki/Portable_Compiled_Format).

## Installation

```shell
dotnet add package PcfFont
```

## Usage

### Create

```csharp
using PcfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var builder = new PcfFontBuilder();
builder.Config.FontAscent = 14;
builder.Config.FontDescent = 2;

builder.Glyphs.Add(new PcfGlyph(
    name: "A",
    encoding: 65,
    scalableWidth: 500,
    characterWidth: 8,
    dimensions: (8, 16),
    offset: (0, -2),
    bitmap: [
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 0, 0],
        [0, 0, 1, 0, 0, 1, 0, 0],
        [0, 0, 1, 0, 0, 1, 0, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 1, 1, 1, 1, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0]
    ]));

builder.Properties.Foundry = "Pixel Font Studio";
builder.Properties.FamilyName = "My Font";
builder.Properties.WeightName = "Medium";
builder.Properties.Slant = "R";
builder.Properties.SetWidthName = "Normal";
builder.Properties.AddStyleName = "Sans Serif";
builder.Properties.PixelSize = 16;
builder.Properties.PointSize = builder.Properties.PixelSize * 10;
builder.Properties.ResolutionX = 75;
builder.Properties.ResolutionY = 75;
builder.Properties.Spacing = "P";
builder.Properties.AverageWidth = Convert.ToInt32(builder.Glyphs.Average(glyph => glyph.CharacterWidth * 10));
builder.Properties.CharsetRegistry = "ISO10646";
builder.Properties.CharsetEncoding = "1";
builder.Properties.GenerateXlfd();

builder.Properties.XHeight = 7;
builder.Properties.CapHeight = 10;
builder.Properties.UnderlinePosition = -2;
builder.Properties.UnderlineThickness = 1;

builder.Properties.FontVersion = "1.0.0";
builder.Properties.Copyright = "Copyright (c) TakWolf";

builder.Save(Path.Combine(outputsDir, "my-font.pcf"));
```

### Load

```csharp
using PcfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var font = PcfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.03.pcf"));
Console.WriteLine($"name: {font.Properties!.Font}");
Console.WriteLine($"size: {font.Properties!.PointSize}");
Console.WriteLine($"ascent: {font.Accelerators!.FontAscent}");
Console.WriteLine($"descent: {font.Accelerators!.FontDescent}");
Console.WriteLine();
foreach (var (encoding, glyphIndex) in font.BdfEncodings!)
{
    var glyphName = font.GlyphNames![glyphIndex];
    var metric = font.Metrics![glyphIndex];
    var bitmap = font.Bitmaps![glyphIndex];
    Console.WriteLine($"char: {char.ConvertFromUtf32(encoding)} ({encoding:X4})");
    Console.WriteLine($"glyphName: {glyphName}");
    Console.WriteLine($"advanceWidth: {metric.CharacterWidth}");
    Console.WriteLine($"dimensions: {metric.Dimensions}");
    Console.WriteLine($"offset: {metric.Offset}");
    foreach (var bitmapRow in bitmap)
    {
        var text = string.Join("", bitmapRow).Replace("0", "  ").Replace("1", "██");
        Console.WriteLine($"{text}*");
    }
    Console.WriteLine();
}
font.Save(Path.Combine(outputsDir, "unifont-17.0.03.pcf"));
```

## References

- [FreeType font driver for PCF fonts](https://github.com/freetype/freetype/tree/master/src/pcf)
- [FontForge - The X11 PCF bitmap font file format](https://fontforge.org/docs/techref/pcf-format.html)
- [The X Font Library](https://www.x.org/releases/current/doc/libXfont/fontlib.html)
- [bdftopcf](https://gitlab.freedesktop.org/xorg/util/bdftopcf)
- [bdftopcf - docs](https://www.x.org/releases/current/doc/man/man1/bdftopcf.1.xhtml)
- [X Logical Font Description Conventions - X Consortium Standard](https://www.x.org/releases/current/doc/xorg-docs/xlfd/xlfd.html)

## License

[MIT License](LICENSE)
