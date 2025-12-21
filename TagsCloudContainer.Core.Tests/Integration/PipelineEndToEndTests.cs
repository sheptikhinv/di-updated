using System.Drawing;
using TagsCloudContainer.Core.CloudRenderers;
using TagsCloudContainer.Core.CoordinateGenerators;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.LayoutBuilders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.Visualizators;
using TagsCloudContainer.Core.WordFilters;
using TagsCloudContainer.Core.WordProcessing;
using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.Tests.Integration;

[TestFixture]
public class PipelineEndToEndTests
{
    private string? _tempInputFile;
    private string? _tempFilterFile;
    private string? _tempOutputFile;

    [SetUp]
    public void SetUp()
    {
        _tempInputFile = Path.Combine(Path.GetTempPath(), $"input_{Guid.NewGuid()}.txt");
        _tempFilterFile = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.txt");
        _tempOutputFile = Path.Combine(Path.GetTempPath(), $"cloud_{Guid.NewGuid()}.png");
    }

    [Test]
    public void Pipeline_ProducesPng_WithConfiguredSize()
    {
        File.WriteAllLines(_tempInputFile!, new[] { "Hello", "world", "world", "of", "TAGS" });
        var boringWords = new List<string> { "of" };
        
        var readers = new IFileReader[] { new TxtFileReader() };
        var processingRules = new IWordProcessingRule[] { new LowerizeWordProcessingRule() };
        var readerFactory = new FileReaderFactory(readers);
        var boringFilter = new SpecifiedBoringWordsFilter(boringWords);
        var processor = new WordsProcessor(boringFilter, processingRules);

        var size = 150;
        var options = new VisualizationOptions
        {
            ImageWidthPx = size,
            ImageHeightPx = size,
            BackgroundColor = Color.White,
            FontColor = Color.Black,
            FontSize = 14f,
            Padding = 32
        };
        var generator = new SpiralCoordinateGenerator(new Point(size / 2, size / 2), 0.6);
        var layoutBuilder = new BasicLayoutBuilder(generator, options);
        var renderer = new BasicCloudRenderer();

        var rawWords = readerFactory.GetReader(_tempInputFile).ReadWords(_tempInputFile);
        var counts = processor.ProcessAndCountWords(rawWords);
        var bitmap = renderer.RenderCloud(layoutBuilder.BuildLayout(counts), options);
        FileSaver.SaveFile(bitmap, _tempOutputFile);
        
        Assert.That(File.Exists(_tempOutputFile), Is.True, "Output file was not created");
        using var tempBitmap = new Bitmap(_tempOutputFile!);
        Assert.That(tempBitmap.Width, Is.EqualTo(size));
        Assert.That(tempBitmap.Height, Is.EqualTo(size));
    }

    [TearDown]
    public void TearDown()
    {
        if (_tempInputFile != null && File.Exists(_tempInputFile))
            File.Delete(_tempInputFile);
        if (_tempFilterFile != null && File.Exists(_tempFilterFile))
            File.Delete(_tempFilterFile);
        if (_tempOutputFile != null && File.Exists(_tempOutputFile))
            File.Delete(_tempOutputFile);
    }
}
