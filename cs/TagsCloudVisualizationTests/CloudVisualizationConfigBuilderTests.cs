using System.Drawing;
using FluentAssertions;
using SixLabors.ImageSharp.PixelFormats;
using TagCloud.Visualisation;
using DrawingRectangle = System.Drawing.Rectangle;

namespace TagCloudTests;

[TestFixture]
public class CloudVisualizationConfigBuilderTests
{
    [Test]
    public void Build_WhenNoOverrides_ShouldUseDefaultValues()
    {
        var builder = new CloudVisualizationConfigBuilder();

        var config = builder.Build();

        config.Width.Should().Be(800);
        config.Height.Should().Be(800);
        config.BackgroundColor.Should().Be(new Rgba32(255, 255, 255));
        config.Center.Should().Be(new Point(400, 400));

        var color = config.RectangleColorProvider(new DrawingRectangle(0, 0, 10, 10), 0);
        color.Should().Be(new Rgba32(100, 100, 100));
    }

    [Test]
    public void WithImageSize_WhenNotSetExplicitly_ShouldOverrideWidthAndHeightAndRecalculateCenter()
    {
        var builder = new CloudVisualizationConfigBuilder();

        var config = builder
            .WithImageSize(1024, 768)
            .Build();

        config.Width.Should().Be(1024);
        config.Height.Should().Be(768);
        config.Center.Should().Be(new Point(512, 384)); 
    }

    [Test]
    public void WithBackground_ShouldOverrideBackgroundColor()
    {
        var builder = new CloudVisualizationConfigBuilder();
        var red = new Rgba32(255, 0, 0);

        var config = builder
            .WithBackground(red)
            .Build();

        config.BackgroundColor.Should().Be(red);
    }

    [Test]
    public void WithCenter_ShouldUseProvidedCenter_EvenIfImageSizeChanges()
    {
        var builder = new CloudVisualizationConfigBuilder();
        var customCenter = new Point(123, 456);

        var config = builder
            .WithImageSize(1000, 1000)
            .WithCenter(customCenter)
            .WithImageSize(2000, 2000) 
            .Build();

        config.Center.Should().Be(customCenter);
    }

    [Test]
    public void WithRectangleColor_ShouldSetConstantColorProvider()
    {
        var builder = new CloudVisualizationConfigBuilder();
        var blue = new Rgba32(0, 0, 255);

        var config = builder
            .WithRectangleColor(blue)
            .Build();

        var color1 = config.RectangleColorProvider(new DrawingRectangle(0, 0, 10, 10), 0);
        var color2 = config.RectangleColorProvider(new DrawingRectangle(5, 5, 20, 20), 42);
        color1.Should().Be(blue);
        color2.Should().Be(blue);
    }

    [Test]
    public void WithRandomRectangleColors_ShouldBeDeterministic_ForSameSeed()
    {
        const int seed = 42;

        var builder1 = new CloudVisualizationConfigBuilder();
        var config1 = builder1
            .WithRandomRectangleColors(seed)
            .Build();

        var builder2 = new CloudVisualizationConfigBuilder();
        var config2 = builder2
            .WithRandomRectangleColors(seed)
            .Build();

        var rect = new DrawingRectangle(0, 0, 10, 10);
        var sequence1 = Enumerable.Range(0, 10)
            .Select(i => config1.RectangleColorProvider(rect, i))
            .ToArray();
        var sequence2 = Enumerable.Range(0, 10)
            .Select(i => config2.RectangleColorProvider(rect, i))
            .ToArray();
        sequence1.Should().BeEquivalentTo(sequence2);
    }

    [Test]
    public void BuilderMethods_ShouldBeChainable()
    {
        var builder = new CloudVisualizationConfigBuilder();

        var config = builder
            .WithImageSize(1200, 900)
            .WithBackground(new Rgba32(10, 20, 30))
            .WithCenter(new Point(100, 200))
            .WithRectangleColor(new Rgba32(200, 100, 50))
            .Build();

        config.Width.Should().Be(1200);
        config.Height.Should().Be(900);
        config.BackgroundColor.Should().Be(new Rgba32(10, 20, 30));
        config.Center.Should().Be(new Point(100, 200));

        var color = config.RectangleColorProvider(new DrawingRectangle(0, 0, 10, 10), 0);
        color.Should().Be(new Rgba32(200, 100, 50));
    }
}
