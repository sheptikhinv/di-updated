using System.Drawing;

namespace TagsCloudContainer.Core.CoordinateGenerators;

public class SpiralCoordinateGenerator : ICoordinateGenerator
{
    private readonly Point _center;
    private double _angleRadians;
    private readonly double _step;

    public SpiralCoordinateGenerator(Point center, double step)
    {
        _center = center;
        _angleRadians = 0;
        _step = step;
    }

    public Point GetNextPosition()
    {
        var radius = _step * _angleRadians;
        var x = _center.X + radius * Math.Cos(_angleRadians);
        var y = _center.Y + radius * Math.Sin(_angleRadians);

        _angleRadians += _step;

        return new Point((int)x, (int)y);
    }
}