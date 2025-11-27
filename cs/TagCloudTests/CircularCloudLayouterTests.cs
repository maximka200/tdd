using System.Drawing;
using FluentAssertions;
using TagCloud;

namespace TagCloudTests;

public class CircularCloudLayouterTests
{
    private CircularCloudLayouter cloudLayouter;
    
    [SetUp]
    public void Setup()
    {
        var center = new Point(0, 0);
        cloudLayouter = new CircularCloudLayouter(center);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(1, 0)]
    [TestCase(0, 1)]
    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(-10, -19)]
    public void PutNextRectangle_WidthOrLengthLessThanOne_ShouldThrowException(int width, int height)
    {
        var rectangleSize = new Size(width, height);

        var act = () => cloudLayouter.PutNextRectangle(rectangleSize);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Размер прямоугольника должен быть положительным");
    }
}