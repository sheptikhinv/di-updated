using System.Drawing;
using System.Drawing.Imaging;
using TagsCloudContainer.Core.CoordinateGenerators;

namespace TagsCloudContainer.Core.Visualizators;

public readonly record struct WordLayout(string Word, Rectangle Bounds, float FontSize);

public class BasicVisualizator : IVisualizator
{
    private readonly record struct CloudBounds(int MinX, int MinY, int MaxX, int MaxY)
    {
        public int Width => MaxX - MinX;
        public int Height => MaxY - MinY;
    }
    
    private readonly ICoordinateGenerator _generator;
    private readonly Random _random;
    private VisualizationOptions _visualizationOptions;

    public BasicVisualizator(ICoordinateGenerator generator, VisualizationOptions visualizationOptions)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _random = new Random();
        _visualizationOptions = visualizationOptions;
    }

    public void DrawWordsToFile(Dictionary<string, int> wordFrequencies, string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        if (wordFrequencies.Count == 0)
        {
            SaveEmptyImage(path);
            return;
        }

        var wordLayouts = CreateWordLayouts(wordFrequencies);
        var bitmap = CreateBitmapForCloud(wordLayouts);

        DrawWordsOnBitmap(bitmap, wordLayouts);
        bitmap.Save(path, ImageFormat.Png);
        bitmap.Dispose();
    }

    private void SaveEmptyImage(string path)
    {
        using var emptyBitmap = new Bitmap(100, 100);
        emptyBitmap.Save(path, ImageFormat.Png);
    }

    private List<WordLayout> CreateWordLayouts(Dictionary<string, int> wordFrequencies)
    {
        var layouts = new List<WordLayout>();
        var maxFrequency = wordFrequencies.Values.Max();
        var sortedWords = wordFrequencies.OrderByDescending(w => w.Value).ToList();

        foreach (var (word, frequency) in sortedWords)
        {
            var fontSize = CalculateFontSize(frequency, maxFrequency);
            var wordSize = GetWordSize(word, fontSize);
            var bounds = FindPositionForWord(wordSize, layouts);

            layouts.Add(new WordLayout(word, bounds, fontSize));
        }

        return layouts;
    }

    private Rectangle FindPositionForWord(Size wordSize, List<WordLayout> existingLayouts)
    {
        Rectangle bounds;

        do
        {
            var position = _generator.GetNextPosition();
            bounds = CreateRectangleAtPosition(position, wordSize);
        } while (IntersectsWithExistingWords(bounds, existingLayouts));

        return bounds;
    }

    private Rectangle CreateRectangleAtPosition(Point position, Size wordSize)
    {
        return new Rectangle(
            position.X - wordSize.Width / 2,
            position.Y - wordSize.Height / 2,
            wordSize.Width,
            wordSize.Height);
    }

    private bool IntersectsWithExistingWords(Rectangle rect, List<WordLayout> layouts)
    {
        foreach (var layout in layouts)
        {
            if (layout.Bounds.IntersectsWith(rect))
                return true;
        }

        return false;
    }

    private Size GetWordSize(string word, float fontSize)
    {
        using var tempBitmap = new Bitmap(1, 1);
        using var graphics = Graphics.FromImage(tempBitmap);
        using var font = new Font("Arial", fontSize, FontStyle.Regular);

        var size = graphics.MeasureString(word, font);

        return new Size(
            (int)Math.Ceiling(size.Width),
            (int)Math.Ceiling(size.Height));
    }

    private float CalculateFontSize(int frequency, int maxFrequency)
    {
        var ratio = (float)frequency / maxFrequency;
        return _visualizationOptions.FontSize + ratio * _visualizationOptions.FontSize * 5;
    }

    private Bitmap CreateBitmapForCloud(List<WordLayout> wordLayouts)
    {
        if (wordLayouts.Count == 0)
            return new Bitmap(100, 100);

        if (_visualizationOptions.ImageSize > 0)
        {
            return new Bitmap(_visualizationOptions.ImageSize, _visualizationOptions.ImageSize);
        }

        var bounds = CalculateCloudBounds(wordLayouts);
        var width = bounds.Width + _visualizationOptions.Padding * 2;
        var height = bounds.Height + _visualizationOptions.Padding * 2;

        return new Bitmap(Math.Max(width, 100), Math.Max(height, 100));
    }

    private void DrawWordsOnBitmap(Bitmap bitmap, List<WordLayout> wordLayouts)
    {
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(_visualizationOptions.BackgroundColor);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        if (wordLayouts.Count == 0)
            return;

        if (_visualizationOptions.ImageSize > 0)
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
        var centerX = _visualizationOptions.ImageSize / 2;
        var centerY = _visualizationOptions.ImageSize / 2;
        
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