using NUnit.Framework;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.Utils;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Tests.Utils;

[TestFixture]
public class WordsProcessorTests
{
    [Test]
    public void ProcessWords_CountsFrequencies_AfterFiltering()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"words_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFile, new[] { "Hello", "world", "WORLD", "of", "C#" });
            var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });
            var filter = new LengthBoringWordsFilter();
            var processor = new WordsProcessor(factory, filter);

            var counts = processor.ProcessWords(tempFile);

            Assert.That(counts.Count, Is.EqualTo(2));
            Assert.That(counts["hello"], Is.EqualTo(1));
            Assert.That(counts["world"], Is.EqualTo(2));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Test]
    public void ProcessWords_CountsFrequencies_WithDifferentCase()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"words_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFile, new[] { "bobik", "Bobik", "boBik", "BOBIK" });
            var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });
            var filter = new LengthBoringWordsFilter();
            var processor = new WordsProcessor(factory, filter);

            var counts = processor.ProcessWords(tempFile);

            Assert.That(counts.Count, Is.EqualTo(1));
            Assert.That(counts["bobik"], Is.EqualTo(4));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
