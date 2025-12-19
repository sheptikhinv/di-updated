namespace TagsCloudContainer.Core.WordFilters;

public interface IBoringWordsFilter
{
    List<string> ExcludeBoringWords(List<string> words);
    List<string> GetBoringWords(List<string> words);
}