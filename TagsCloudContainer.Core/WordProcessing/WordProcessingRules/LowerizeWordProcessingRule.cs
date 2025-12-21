namespace TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

public class LowerizeWordProcessingRule : IWordProcessingRule
{
    public IEnumerable<string> Process(IEnumerable<string> words)
    {
        return words.Select(w => w.ToLower());
    }
}