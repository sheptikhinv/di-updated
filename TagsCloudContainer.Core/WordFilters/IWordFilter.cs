namespace TagsCloudContainer.Core.WordFilters;

public interface IWordFilter
{
    List<string> ExcludeBoringWords(List<string> words);
    List<string> GetBoringWords(List<string> words);
}