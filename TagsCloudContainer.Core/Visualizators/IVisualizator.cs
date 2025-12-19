namespace TagsCloudContainer.Core.Visualizators;

public interface IVisualizator
{
    void DrawWordsToFile(Dictionary<string, int> wordFrequencies, string path);
}