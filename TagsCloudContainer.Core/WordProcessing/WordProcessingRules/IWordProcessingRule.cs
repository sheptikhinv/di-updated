namespace TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

public interface IWordProcessingRule
{
    public IEnumerable<string> Process(IEnumerable<string> words);
}