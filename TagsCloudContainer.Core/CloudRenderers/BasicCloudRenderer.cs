using System.Drawing;
using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.CloudRenderers;

public class BasicCloudRenderer : ICloudRenderer
{
    private readonly Random _random;

    public BasicCloudRenderer()
    {
        _random = new Random();
    }

    public Bitmap RenderCloud(List<WordLayout> wordLayouts, VisualizationOptions visualizationOptions)
    {
        var bitmap = CreateBitmapForCloud(wordLayouts, visualizationOptions.ImageWidthPx,
            visualizationOptions.ImageHeightPx, visualizationOptions.Padding);
        DrawWordsOnBitmap(bitmap, wordLayouts, visualizationOptions);
        return bitmap;
    }

    private Bitmap CreateBitmapForCloud(List<WordLayout> wordLayouts, int? imageWidthPx, int? imageHeightPx,
        int padding)
    {
        if (wordLayouts.Count == 0)
            return new Bitmap(100, 100);

        if (imageWidthPx.HasValue && imageHeightPx.HasValue)
        {
            return new Bitmap(
                Math.Max(imageWidthPx.Value, 100),
                Math.Max(imageHeightPx.Value, 100));
        }

        var bounds = CalculateCloudBounds(wordLayouts);
        var width = imageWidthPx ?? bounds.Width + padding * 2;
        var height = imageHeightPx ?? bounds.Height + padding * 2;

        return new Bitmap(Math.Max(width, 100), Math.Max(height, 100));
    }

    private void DrawWordsOnBitmap(Bitmap bitmap, List<WordLayout> wordLayouts,
        VisualizationOptions visualizationOptions)
    {
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(visualizationOptions.BackgroundColor);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        if (wordLayouts.Count == 0)
            return;

        if (visualizationOptions.ImageWidthPx > 0)
        {
            DrawWordsWithFixedSize(graphics, wordLayouts, visualizationOptions);
        }
        else
        {
            var cloudBounds = CalculateCloudBounds(wordLayouts);
            foreach (var layout in wordLayouts)
            {
                DrawSingleWord(graphics, layout, cloudBounds, visualizationOptions);
            }
        }
    }

    private void DrawWordsWithFixedSize(Graphics graphics, List<WordLayout> wordLayouts,
        VisualizationOptions visualizationOptions)
    {
        var cloudBounds = CalculateCloudBounds(wordLayouts);
        var centerX = visualizationOptions.ImageWidthPx.Value / 2;
        var centerY = visualizationOptions.ImageHeightPx.Value / 2;

        var cloudCenterX = (cloudBounds.MinX + cloudBounds.MaxX) / 2;
        var cloudCenterY = (cloudBounds.MinY + cloudBounds.MaxY) / 2;

        foreach (var layout in wordLayouts)
        {
            using var font = new Font(visualizationOptions.FontFamily, layout.FontSize, FontStyle.Regular);
            using var brush = new SolidBrush(visualizationOptions.FontColor ?? GetRandomColor());

            var adjustedX = layout.Bounds.Left - cloudCenterX + centerX;
            var adjustedY = layout.Bounds.Top - cloudCenterY + centerY;

            graphics.DrawString(layout.Word, font, brush, new Point(adjustedX, adjustedY));
        }
    }

    private void DrawSingleWord(Graphics graphics, WordLayout layout, CloudBounds cloudBounds,
        VisualizationOptions visualizationOptions)
    {
        using var font = new Font(visualizationOptions.FontFamily, layout.FontSize, FontStyle.Regular);
        using var brush = new SolidBrush(visualizationOptions.FontColor ?? GetRandomColor());

        var adjustedPosition = CalculateDrawPosition(layout.Bounds, cloudBounds, visualizationOptions.Padding);
        graphics.DrawString(layout.Word, font, brush, adjustedPosition);
    }

    private Point CalculateDrawPosition(Rectangle bounds, CloudBounds cloudBounds, int padding)
    {
        return new Point(
            bounds.Left - cloudBounds.MinX + padding,
            bounds.Top - cloudBounds.MinY + padding);
    }

    private CloudBounds CalculateCloudBounds(List<WordLayout> wordLayouts)
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        foreach (var layout in wordLayouts)
        {
            var rect = layout.Bounds;
            minX = Math.Min(minX, rect.Left);
            minY = Math.Min(minY, rect.Top);
            maxX = Math.Max(maxX, rect.Right);
            maxY = Math.Max(maxY, rect.Bottom);
        }

        return new CloudBounds(minX, minY, maxX, maxY);
    }

    private Color GetRandomColor()
    {
        return Color.FromArgb(
            _random.Next(256),
            _random.Next(256),
            _random.Next(256));
    }
}