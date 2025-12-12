using TagsCloudContainer.Core.FileReaders;

namespace TagsCloudContainer.Core.WordFilters;

public class FileWordFilter : IWordFilter
{
    private readonly IFileReader _reader;
    private List<string> _wordsToExclude;

    public FileWordFilter(IFileReader reader, string filePath)
    {
        _reader = reader;
        _wordsToExclude = _reader.GetWords(filePath).Distinct().ToList();
    }

    public List<string> ExcludeBoringWords(List<string> words)
    {
        var result = words.Except(_wordsToExclude).ToList();
        return result;
    }

    public List<string> GetBoringWords(List<string> words)
    {
        var result = words.Where(w => _wordsToExclude.Contains(w)).ToList();
        return result;
    }
}