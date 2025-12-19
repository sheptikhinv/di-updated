namespace TagsCloudContainer.Core.WordFilters;

public class LengthBoringWordsFilter : IBoringWordsFilter
{
    private const int MinWordLength = 3;

    public List<string> ExcludeBoringWords(List<string> words)
    {
        var result = words.Where(w => w.Length >= MinWordLength).Select(w => w.ToLower()).ToList();
        return result;
    }

    public List<string> GetBoringWords(List<string> words)
    {
        var result = words.Where(w => w.Length < MinWordLength).ToList();
        return result;
    }
}