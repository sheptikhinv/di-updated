using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Tests.WordFilters;

[TestFixture]
public class SpecifiedBoringWordsFilterTests
{
    [Test]
    public void ExcludeBoringWords_RemovesListedWords_Test()
    {
        var filterWords = new List<string> { "the", "and", "of" };
        var filter = new SpecifiedBoringWordsFilter(filterWords);

        var input = new List<string> { "the", "banana", "and", "world", "of" };

        var result = filter.ExcludeBoringWords(input);

        CollectionAssert.AreEquivalent(new[] { "banana", "world" }, result);
    }

    [Test]
    public void GetBoringWords_ReturnsIntersectionWithList_Test()
    {
        var filterWords = new List<string> { "stop", "boring" };
        var filter = new SpecifiedBoringWordsFilter(filterWords);

        var input = new List<string> { "fun", "boring", "STOP", "stop" };

        var boring = filter.GetBoringWords(input);

        CollectionAssert.AreEquivalent(new[] { "boring", "stop" }, boring);
    }

    [Test]
    public void ExcludeBoringWords_DoesntDeleteDuplicates_Test()
    {
        var filter = new SpecifiedBoringWordsFilter(new List<string>());
        var input = new List<string> { "the", "the", "the" };
        var result = filter.ExcludeBoringWords(input);
        Assert.That(result, Has.Count.EqualTo(3));
    }
}