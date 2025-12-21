using System.Drawing;

namespace TagsCloudContainer.Core.CoordinateGenerators;

public class SpiralCoordinateGenerator : ICoordinateGenerator
{
    private readonly Point _center;
    private double _angleRadians;
    private readonly double _stepRadians;

    public SpiralCoordinateGenerator(Point center, double stepRadians)
    {
        _center = center;
        _angleRadians = 0;
        _stepRadians = stepRadians;
    }

    public Point GetNextPosition()
    {
        var radius = _stepRadians * _angleRadians;
        var x = _center.X + radius * Math.Cos(_angleRadians);
        var y = _center.Y + radius * Math.Sin(_angleRadians);

        _angleRadians += _stepRadians;

        return new Point((int)x, (int)y);
    }
}