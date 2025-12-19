using System.Drawing;
using NUnit.Framework;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.Tests.Visualizators;

[TestFixture]
public class BasicVisualizatorTests
{
    private static SpiralCoordinateGenerator CreateGenerator(int size)
    {
        var center = new Point(size / 2, size / 2);
        return new SpiralCoordinateGenerator(center, 0.5);
    }

    [Test]
    public void DrawWordsToFile_Empty_Writes100x100Png()
    {
        var options = new VisualizationOptions
        {
            ImageSize = 256,
            BackgroundColor = Color.White,
            FontColor = Color.Black,
            FontSize = 12f
        };
        var generator = CreateGenerator(options.ImageSize);
        var visualizator = new BasicVisualizator(generator, options);

        var path = Path.Combine(Path.GetTempPath(), $"cloud_empty_{Guid.NewGuid()}.png");
        try
        {
            visualizator.DrawWordsToFile(new Dictionary<string, int>(), path);
            Assert.That(File.Exists(path), Is.True);

            using var bmp = new Bitmap(path);
            Assert.That(bmp.Width, Is.EqualTo(100));
            Assert.That(bmp.Height, Is.EqualTo(100));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Test]
    public void DrawWordsToFile_WithWords_UsesFixedImageSize()
    {
        var options = new VisualizationOptions
        {
            ImageSize = 200,
            BackgroundColor = Color.White,
            FontColor = Color.Black,
            FontSize = 16f
        };
        var generator = CreateGenerator(options.ImageSize);
        var visualizator = new BasicVisualizator(generator, options);

        var path = Path.Combine(Path.GetTempPath(), $"cloud_fixed_{Guid.NewGuid()}.png");
        try
        {
            var words = new Dictionary<string, int> { { "hello", 2 }, { "world", 1 } };
            visualizator.DrawWordsToFile(words, path);

            using var bmp = new Bitmap(path);
            Assert.That(bmp.Width, Is.EqualTo(200));
            Assert.That(bmp.Height, Is.EqualTo(200));

            // ensure not a blank background image
            var hasNonBackground = false;
            for (int y = 0; y < bmp.Height && !hasNonBackground; y += 5)
            {
                for (int x = 0; x < bmp.Width && !hasNonBackground; x += 5)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.ToArgb() != options.BackgroundColor.ToArgb()) hasNonBackground = true;
                }
            }
            Assert.That(hasNonBackground, Is.True, "Expected some drawn text on the bitmap");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Test]
    public void DrawWordsToFile_AutoSize_ImageAtLeast100()
    {
        var options = new VisualizationOptions
        {
            ImageSize = 0, // auto size
            BackgroundColor = Color.White,
            FontColor = Color.Black,
            FontSize = 18f,
            Padding = 16
        };
        var generator = new SpiralCoordinateGenerator(new Point(0, 0), 0.8);
        var visualizator = new BasicVisualizator(generator, options);

        var path = Path.Combine(Path.GetTempPath(), $"cloud_auto_{Guid.NewGuid()}.png");
        try
        {
            var words = new Dictionary<string, int> { { "alpha", 3 }, { "beta", 2 }, { "gamma", 1 } };
            visualizator.DrawWordsToFile(words, path);

            using var bmp = new Bitmap(path);
            Assert.That(bmp.Width, Is.GreaterThan(100));
            Assert.That(bmp.Height, Is.GreaterThan(100));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
