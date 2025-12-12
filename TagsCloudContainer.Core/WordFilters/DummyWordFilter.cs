namespace TagsCloudContainer.Core.WordFilters;

public class DummyWordFilter : IWordFilter
{
    private const int MinWordLength = 3;

    public List<string> ExcludeBoringWords(List<string> words)
    {
        var result = words.Where(w => w.Length >= MinWordLength).ToList();
        return result;
    }

    public List<string> GetBoringWords(List<string> words)
    {
        var result = words.Where(w => w.Length < MinWordLength).ToList();
        return result;
    }
}