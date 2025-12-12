namespace TagsCloudContainer.Core.Utils;

public static class WordCounter
{
    public static Dictionary<string, int> CountWords(List<string> words)
    {
        var result = new Dictionary<string, int>();
        foreach (var word in words.Where(word => !result.TryAdd(word, 1)))
        {
            result[word]++;
        }
        return result;
    }
}