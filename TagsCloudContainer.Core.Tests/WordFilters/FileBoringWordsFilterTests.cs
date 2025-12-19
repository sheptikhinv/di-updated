using NUnit.Framework;
using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Tests.WordFilters;

[TestFixture]
public class FileBoringWordsFilterTests
{
    [Test]
    public void ExcludeBoringWords_RemovesWordsListedInFile()
    {
        var tempFilterFile = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFilterFile, new[] { "the", "and", "of" });
            var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });
            var filter = new FileBoringWordsFilter(factory, tempFilterFile);

            var input = new List<string> { "the", "banana", "and", "world", "of" };

            var result = filter.ExcludeBoringWords(input);

            CollectionAssert.AreEquivalent(new[] { "banana", "world" }, result);
        }
        finally
        {
            if (File.Exists(tempFilterFile)) File.Delete(tempFilterFile);
        }
    }

    [Test]
    public void GetBoringWords_ReturnsIntersectionWithFileList()
    {
        var tempFilterFile = Path.Combine(Path.GetTempPath(), $"filter_{Guid.NewGuid()}.txt");
        try
        {
            File.WriteAllLines(tempFilterFile, new[] { "stop", "boring" });
            var factory = new FileReaderFactory(new IFileReader[] { new BasicFileReader() });
            var filter = new FileBoringWordsFilter(factory, tempFilterFile);

            var input = new List<string> { "fun", "boring", "STOP", "Stop" };

            var boring = filter.GetBoringWords(input.Select(s => s.ToLower()).ToList());

            CollectionAssert.AreEquivalent(new[] { "boring", "stop" }, boring);
        }
        finally
        {
            if (File.Exists(tempFilterFile)) File.Delete(tempFilterFile);
        }
    }
}
