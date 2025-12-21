namespace TagsCloudContainer.Core.WordFilters;

public class SpecifiedBoringWordsFilter : IBoringWordsFilter
{
    private readonly List<string> _wordsToExclude;

    public SpecifiedBoringWordsFilter(List<string> wordsToExclude)
    {
        _wordsToExclude = wordsToExclude;
    }

    public List<string> ExcludeBoringWords(List<string> words)
    {
        var result = words.Where(w => !_wordsToExclude.Contains(w)).ToList();
        return result;
    }

    public List<string> GetBoringWords(List<string> words)
    {
        var result = words.Where(w => _wordsToExclude.Contains(w)).Distinct().ToList();
        return result;
    }
}