using System.Drawing;
using NUnit.Framework;
using TagsCloudContainer.Core.CoordinateGenerators;

namespace TagsCloudContainer.Core.Tests.CoordinateGenerators;

[TestFixture]
public class SpiralCoordinateGeneratorTests
{
    [Test]
    public void GetNextPosition_FirstCall_ReturnsCenter()
    {
        var center = new Point(100, 200);
        var gen = new SpiralCoordinateGenerator(center, 1.0);

        var p = gen.GetNextPosition();

        Assert.That(p, Is.EqualTo(center));
    }

    [Test]
    public void GetNextPosition_Eventually_MovesAwayFromCenter()
    {
        var center = new Point(0, 0);
        var gen = new SpiralCoordinateGenerator(center, 0.5);

        var moved = false;
        for (int i = 0; i < 1000; i++)
        {
            var p = gen.GetNextPosition();
            if (p != center)
            {
                moved = true;
                break;
            }
        }

        Assert.That(moved, Is.True, "Generator never moved away from center within 1000 steps");
    }

    [Test]
    public void GetNextPosition_ProducesDifferentPoints_OverTime()
    {
        var center = new Point(0, 0);
        var gen = new SpiralCoordinateGenerator(center, 1.0);

        var set = new HashSet<Point>();
        for (int i = 0; i < 2; i++)
            set.Add(gen.GetNextPosition());

        Assert.That(set.Count, Is.GreaterThan(1), "Expected more than one unique point after 2 calls");
    }
}
