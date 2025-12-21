using System.Drawing;

namespace TagsCloudContainer.Core.Visualizators;

public record VisualizationOptions
{
    public int Padding { get; init; } = 64;
    public float FontSize { get; init; } = 12f;
    public Color BackgroundColor { get; init; } = Color.Black;
    public Color? FontColor { get; init; }
    public int ImageWidthPx { get; init; } = 300;
    public int ImageHeightPx { get; init; } = 300;
}