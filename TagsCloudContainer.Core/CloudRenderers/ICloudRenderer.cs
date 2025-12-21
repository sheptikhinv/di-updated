using System.Drawing;
using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.CloudRenderers;

public interface ICloudRenderer
{
    Bitmap RenderCloud(List<WordLayout> wordLayouts, VisualizationOptions options);
}