using System.Drawing;

namespace TagsCloudContainer.Core.CoordinateGenerators;

public interface ICoordinateGenerator
{
    Point GetNextPosition();
}