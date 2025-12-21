namespace TagsCloudContainer.Core.DTOs;

public readonly record struct CloudBounds(int MinX, int MinY, int MaxX, int MaxY)
{
    public int Width { get; } = MaxX - MinX;
    public int Height { get; } = MaxY - MinY;
}