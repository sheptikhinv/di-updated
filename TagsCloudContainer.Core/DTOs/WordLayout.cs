using System.Drawing;

namespace TagsCloudContainer.Core.DTOs;

public readonly record struct WordLayout(string Word, Rectangle Bounds, float FontSize);