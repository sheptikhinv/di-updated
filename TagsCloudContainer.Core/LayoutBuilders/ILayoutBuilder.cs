using TagsCloudContainer.Core.DTOs;

namespace TagsCloudContainer.Core.LayoutBuilders;

public interface ILayoutBuilder
{
    List<WordLayout> BuildLayout(Dictionary<string, int> wordFrequencies);
}