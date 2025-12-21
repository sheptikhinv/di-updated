using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Tests.WordFilters;

[TestFixture]
public class LengthBoringWordsFilterTests
{
    [Test]
    public void ExcludeBoringWords_RemovesShorterThan3_Test()
    {
        var filter = new LengthBoringWordsFilter();
        var input = new List<string> { "I", "am", "Ok", "nice", "WORLD" };

        var result = filter.ExcludeBoringWords(input);

        CollectionAssert.AreEquivalent(new[] { "nice", "world" }, result);
    }

    [Test]
    public void GetBoringWords_ReturnsShortOnesOnly_Test()
    {
        var filter = new LengthBoringWordsFilter();
        var input = new List<string> { "a", "bb", "ccc", "dddd" };

        var result = filter.GetBoringWords(input);

        CollectionAssert.AreEquivalent(new[] { "a", "bb" }, result);
    }
}
