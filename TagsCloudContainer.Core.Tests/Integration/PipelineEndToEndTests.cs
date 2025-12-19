using System.Drawing;
using NUnit.Framework;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Tests.Integration;

[TestFixture]
public class PipelineEndToEndTests
{
    [Test]
    public void Pipeline_ProducesPng_WithConfiguredSize()
    {
        var tempInput = Path.Combine(Path.GetTempPath(), $"input_{Guid.NewGuid()}.txt");
        var tempFilter = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.txt");
        var output = Path.Combine(Path.GetTempPath(), $"cloud_{Guid.NewGuid()}.png");
        try
        {
            File.WriteAllLines(tempInput, new[] { "Hello", "world", "world", "of", "TAGS" });
            File.WriteAllLines(tempFilter, new[] { "of" });
            
            var readers = new IFileReader[] { new BasicFileReader() };
            var readerFactory = new FileReaderFactory(readers);
            var boringFilter = new FileBoringWordsFilter(readerFactory, tempFilter);
            var processor = new WordsProcessor(readerFactory, boringFilter);

            var size = 150;
            var options = new VisualizationOptions
            {
                ImageSize = size,
                BackgroundColor = Color.White,
                FontColor = Color.Black,
                FontSize = 14f,
                Padding = 32
            };
            var generator = new SpiralCoordinateGenerator(new Point(size / 2, size / 2), 0.6);
            var visualizator = new BasicVisualizator(generator, options);

            var counts = processor.ProcessWords(tempInput);
            visualizator.DrawWordsToFile(counts, output);

            Assert.That(File.Exists(output), Is.True, "Output file was not created");
            using var bmp = new Bitmap(output);
            Assert.That(bmp.Width, Is.EqualTo(size));
            Assert.That(bmp.Height, Is.EqualTo(size));
        }
        finally
        {
            if (File.Exists(tempInput)) File.Delete(tempInput);
            if (File.Exists(tempFilter)) File.Delete(tempFilter);
            if (File.Exists(output)) File.Delete(output);
        }
    }
}
