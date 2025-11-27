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
        spiral = new Spiral(center);
    }

    [Test]
    public void TryAddRectangleInCenter_InCenter_ShouldPlaceFirstRectangle_()
    {
        var size = new Size(20, 10);
        
        var success = spiral.TryAddRectangleInCenter(size, out var rect);
        
        success.Should().BeTrue();
        GetCenter(rect).Should().Be(center);
        rect.Size.Should().Be(size);
    }

    [Test]
    public void TryAddRectangleInCenter_WhenRectangleAlreadyPlaced_ShouldReturnFalse()
    {
        var size = new Size(20, 10);
        spiral.TryAddRectangleInCenter(size, out _);
        
        var success = spiral.TryAddRectangleInCenter(size, out var secondRect);
        
        success.Should().BeFalse();
        secondRect.Should().Be(Rectangle.Empty);
    }

    [Test]
    public void AddRectangle_WithRequestedSize_ShouldReturnRectangle()
    {
        var size = new Size(30, 15);
        
        var rect = spiral.AddRectangle(size);
        
        rect.Size.Should().Be(size);
    }

    [Test]
    public void AddRectangle_WithPreviouslyPlacedRectangles_ShouldNotIntersect()
    {
        var size = new Size(20, 10);
        spiral.TryAddRectangleInCenter(size, out var first);
        var rectangles = new List<Rectangle> { first };
        
        for (var i = 0; i < 20; i++)
        {
            var rect = spiral.AddRectangle(size);
            rectangles.Add(rect);
        }
        
        foreach (var r1 in rectangles)
        foreach (var r2 in rectangles.Where(r2 => r1 != r2))
        {
            r1.IntersectsWith(r2).Should().BeFalse();
        }
    }

    #region Helpers
    
    private static Point GetCenter(Rectangle rect) =>
        new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

    private static double Distance(Point a, Point b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
    
    #endregion
}
