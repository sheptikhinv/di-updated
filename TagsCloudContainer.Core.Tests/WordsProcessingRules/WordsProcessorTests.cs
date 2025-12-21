using TagsCloudContainer.Core.WordFilters;
using TagsCloudContainer.Core.WordProcessing;
using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.Tests.WordsProcessingRules;

[TestFixture]
public class WordsProcessorTests
{
    [Test]
    public void ReadProcessAndCount_ShouldApplyRules_Test()
    {
        var input = new List<string> { "I", "am", "Ok", "nice", "WORLD" };
        var wordProcessor = new WordsProcessor(new SpecifiedBoringWordsFilter([]),
            new List<IWordProcessingRule> { new LowerizeWordProcessingRule() });
        var result = wordProcessor.ProcessAndCountWords(input);
        var keys = result.Keys.ToList();
        CollectionAssert.AreEquivalent(new[] { "i", "am", "ok", "nice", "world" }, keys);
    }

    [Test]
    public void ReadProcessAndCount_ShouldApplyFilters_Test()
    {
        var input = new List<string> { "i", "am", "ok", "nice", "world" };
        var exclude = new List<string> { "am", "ok" };
        var wordProcessor = new WordsProcessor(new SpecifiedBoringWordsFilter(exclude),
            new List<IWordProcessingRule>());
        var result = wordProcessor.ProcessAndCountWords(input);
        var keys = result.Keys.ToList();
        CollectionAssert.AreEquivalent(new[] { "i", "nice", "world" }, keys);
    }

    [Test]
    public void ReadProcessAndCount_ShouldCountWords_Test()
    {
        var input = new List<string> { "first", "first", "first", "second" };
        var wordProcessor = new WordsProcessor(new SpecifiedBoringWordsFilter([]),
            new List<IWordProcessingRule>());
        var result = wordProcessor.ProcessAndCountWords(input);
        Assert.Multiple(() =>
        {
            Assert.That(result["first"], Is.EqualTo(3));
            Assert.That(result["second"], Is.EqualTo(1));
        });
    }
}