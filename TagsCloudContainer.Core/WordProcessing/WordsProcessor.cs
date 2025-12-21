using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.WordFilters;
using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.WordProcessing;

public class WordsProcessor
{
    private readonly IBoringWordsFilter _boringWordsFilter;
    private readonly IEnumerable<IWordProcessingRule> _wordProcessingRules;

    public WordsProcessor(IBoringWordsFilter boringWordsFilter,
        IEnumerable<IWordProcessingRule> wordProcessingRules)
    {
        _boringWordsFilter = boringWordsFilter;
        _wordProcessingRules = wordProcessingRules;
    }

    public Dictionary<string, int> ProcessAndCountWords(IEnumerable<string> words)
{
    var processedWords = words;
    
    foreach (var rule in _wordProcessingRules)
    {
        processedWords = rule.Process(processedWords);
    }

    var filteredWords = _boringWordsFilter.ExcludeBoringWords(processedWords.ToList());
    var result = CountWords(filteredWords);
    return result;
}

    private Dictionary<string, int> CountWords(List<string> words)
    {
        var result = new Dictionary<string, int>();
        foreach (var word in words)
        {
            var exists = result.TryGetValue(word, out var frequency);
            if (exists)
                result[word] = frequency + 1;
            else
                result[word] = 1;
        }

        return result;
    }
}