using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.WordFilters;
using TagsCloudContainer.Core.WordProcessing.WordProcessingRules;

namespace TagsCloudContainer.Core.WordProcessing;

public class WordsProcessor
{
    private readonly FileReaderFactory _readerFactory;
    private readonly IBoringWordsFilter _boringWordsFilter;
    private readonly IEnumerable<IWordProcessingRule> _wordProcessingRules;

    public WordsProcessor(FileReaderFactory readerFactory, IBoringWordsFilter boringWordsFilter,
        IEnumerable<IWordProcessingRule> wordProcessingRules)
    {
        _readerFactory = readerFactory;
        _boringWordsFilter = boringWordsFilter;
        _wordProcessingRules = wordProcessingRules;
    }

    public Dictionary<string, int> ReadProcessAndCountWords(string filePath)
    {
        var readWords = _readerFactory.GetReader(filePath).ReadWords(filePath);

        IEnumerable<string> words = readWords;
        foreach (var rule in _wordProcessingRules)
        {
            rule.Process(words);
        }

        var filteredWords = _boringWordsFilter.ExcludeBoringWords(words.ToList());
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