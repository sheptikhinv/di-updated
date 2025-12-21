using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.Tests.WordsProcessingRules;

[TestFixture]
public class LowerizeWordProcessingRuleTests
{
    [Test]
    public void Process_WordsReturnsLowerized_Test()
    {
        var rule = new LowerizeWordProcessingRule();
        var words = new List<string> { "I", "am", "Ok", "nice", "WORLD" };
        var result = rule.Process(words);

        CollectionAssert.AreEquivalent(new[] { "i", "am", "ok", "nice", "world" }, result);
    }
}