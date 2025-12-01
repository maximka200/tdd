using System.Drawing;
using FluentAssertions;
using TagCloud;

namespace TagCloudTests;

[TestFixture]
public class SpiralTests
{
    private Point center;
    private Spiral spiral;

    [SetUp]
    public void SetUp()
    {
        center = new Point(0, 0);
        spiral = new Spiral(center, angleStep: 0.1, radiusСoeff: 1.0);
    }

    [Test]
    public void GetNextPoint_ShouldReturnDifferentPoints()
    {
        var points = new HashSet<Point>();

        for (var i = 0; i < 50; i++)
            points.Add(spiral.GetNextPoint());

        points.Count.Should().BeGreaterThan(1);
    }
    
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public void GetNextPoint_StepsLessThanZeroOrEqual_ShouldThrowException(int steps)
    {
        var act = () => spiral.GetNextPoint(steps);
        
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage($"steps ('{steps}') must be a non-negative and non-zero value. (Parameter 'steps')\nActual value was {steps}.");
    }
}