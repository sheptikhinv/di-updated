using TagsCloudContainer.Core.FileReaders;
using TagsCloudContainer.Core.WordFilters;

namespace TagsCloudContainer.Core.Utils;

public class WordsProcessor
{
    private readonly FileReaderFactory _readerFactory;
    private readonly IBoringWordsFilter _boringWordsFilter;

    public WordsProcessor(FileReaderFactory readerFactory, IBoringWordsFilter boringWordsFilter)
    {
        _readerFactory = readerFactory;
        _boringWordsFilter = boringWordsFilter;
    }
    
    public Dictionary<string, int> ProcessWords(string filePath)
    {
        var words = _readerFactory.GetReader(filePath).GetWords(filePath)
            .Select(w => w.ToLower()).ToList();
        var filteredWords = _boringWordsFilter.ExcludeBoringWords(words);
        
        var result = new Dictionary<string, int>();
        foreach (var word in filteredWords)
        {
            result.TryAdd(word, 0);
            result[word]++;
        }

        return result;
    }
}