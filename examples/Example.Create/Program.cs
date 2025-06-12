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
