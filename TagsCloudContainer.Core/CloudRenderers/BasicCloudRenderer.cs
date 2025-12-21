using System.Drawing;
using TagsCloudContainer.Core.DTOs;
using TagsCloudContainer.Core.Visualizators;

namespace TagsCloudContainer.Core.CloudRenderers;

public class BasicCloudRenderer : ICloudRenderer
{
    private readonly Random _random;
    private readonly VisualizationOptions _visualizationOptions;

    public BasicCloudRenderer(VisualizationOptions visualizationOptions)
    {
        _random = new Random();
        _visualizationOptions = visualizationOptions;
    }

    public Bitmap RenderCloud(List<WordLayout> wordLayouts)
    {
        var bitmap = CreateBitmapForCloud(wordLayouts);
        DrawWordsOnBitmap(bitmap, wordLayouts);
        return bitmap;
    }

    private Bitmap CreateBitmapForCloud(List<WordLayout> wordLayouts)
    {
        if (wordLayouts.Count == 0)
            return new Bitmap(100, 100);

        var height = 0;
        var width = 0;

        if (_visualizationOptions.ImageWidthPx > 0)
            width = _visualizationOptions.ImageWidthPx;

        if (_visualizationOptions.ImageHeightPx > 0)
            height = _visualizationOptions.ImageHeightPx;

        var bounds = CalculateCloudBounds(wordLayouts);
        if (width == 0)
            width = bounds.Width + _visualizationOptions.Padding * 2;
        if (height == 0)
            height = bounds.Height + _visualizationOptions.Padding * 2;

        return new Bitmap(Math.Max(width, 100), Math.Max(height, 100));
    }

    private void DrawWordsOnBitmap(Bitmap bitmap, List<WordLayout> wordLayouts)
    {
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(_visualizationOptions.BackgroundColor);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        if (wordLayouts.Count == 0)
            return;

        if (_visualizationOptions.ImageWidthPx > 0)
        {
            DrawWordsWithFixedSize(graphics, wordLayouts);
        }
        else
        {
            var cloudBounds = CalculateCloudBounds(wordLayouts);
            foreach (var layout in wordLayouts)
            {
                DrawSingleWord(graphics, layout, cloudBounds);
            }
        }
    }

    private void DrawWordsWithFixedSize(Graphics graphics, List<WordLayout> wordLayouts)
    {
        var cloudBounds = CalculateCloudBounds(wordLayouts);
        var centerX = _visualizationOptions.ImageWidthPx / 2;
        var centerY = _visualizationOptions.ImageWidthPx / 2;

        var cloudCenterX = (cloudBounds.MinX + cloudBounds.MaxX) / 2;
        var cloudCenterY = (cloudBounds.MinY + cloudBounds.MaxY) / 2;

        foreach (var layout in wordLayouts)
        {
            using var font = new Font("Arial", layout.FontSize, FontStyle.Regular);
            using var brush = new SolidBrush(_visualizationOptions.FontColor ?? GetRandomColor());

            var adjustedX = layout.Bounds.Left - cloudCenterX + centerX;
            var adjustedY = layout.Bounds.Top - cloudCenterY + centerY;

            graphics.DrawString(layout.Word, font, brush, new Point(adjustedX, adjustedY));
        }
    }

    private void DrawSingleWord(Graphics graphics, WordLayout layout, CloudBounds cloudBounds)
    {
        using var font = new Font("Arial", layout.FontSize, FontStyle.Regular);
        using var brush = new SolidBrush(_visualizationOptions.FontColor ?? GetRandomColor());

        var adjustedPosition = CalculateDrawPosition(layout.Bounds, cloudBounds);
        graphics.DrawString(layout.Word, font, brush, adjustedPosition);
    }

    private Point CalculateDrawPosition(Rectangle bounds, CloudBounds cloudBounds)
    {
        return new Point(
            bounds.Left - cloudBounds.MinX + _visualizationOptions.Padding,
            bounds.Top - cloudBounds.MinY + _visualizationOptions.Padding);
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