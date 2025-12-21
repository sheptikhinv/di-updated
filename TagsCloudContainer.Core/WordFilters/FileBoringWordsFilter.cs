using TagsCloudContainer.Core.FileReaders;

namespace TagsCloudContainer.Core.WordFilters;

public class FileBoringWordsFilter : IBoringWordsFilter
{
    private readonly List<string> _wordsToExclude;

    public FileBoringWordsFilter(FileReaderFactory readerFactory, string filePath)
    {
        _wordsToExclude = readerFactory
            .GetReader(filePath)
            .ReadWords(filePath)
            .Distinct()
            .Select(w => w.ToLower())
            .ToList();
    }

    public List<string> ExcludeBoringWords(List<string> words)
    {
        var result = words.Except(_wordsToExclude).ToList();
        return result;
    }

    public List<string> GetBoringWords(List<string> words)
    {
        var result = words.Where(w => _wordsToExclude.Contains(w)).Distinct().ToList();
        return result;
    }
}