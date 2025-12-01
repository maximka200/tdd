using SixLabors.ImageSharp.PixelFormats;
using DrawingRectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;

namespace TagCloud.Visualisation;

public class CloudVisualizationConfigBuilder
{
    private int width = 800;
    private int height = 800;
    private Rgba32 backgroundColor = SixLabors.ImageSharp.Color.White;
    private Point? center;
    private Func<DrawingRectangle, int, Rgba32> rectangleColorProvider =
        (_, _) => new Rgba32(100, 100, 100
            );

    public CloudVisualizationConfigBuilder WithImageSize(int inputWidth, int inputHeight)
    {
        width = inputWidth;
        height = inputHeight;
        return this;
    }

    public CloudVisualizationConfigBuilder WithBackground(Rgba32 color)
    {
        backgroundColor = color; 
        return this;
    }

    public CloudVisualizationConfigBuilder WithCenter(Point inputCenter)
    {
        center = inputCenter; 
        return this;
    }

    public CloudVisualizationConfigBuilder WithRectangleColor(Rgba32 color)
    {
        rectangleColorProvider = (_, _) => color;
        return this;
    }

    public CloudVisualizationConfigBuilder WithRandomRectangleColors(int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        rectangleColorProvider = (_, _) =>
            new Rgba32(
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256)
                );
        return this;
    }
    
    public CloudVisualizationConfig Build()
    {
        var actualCenter = center ?? new Point(width / 2, height / 2);
        return new CloudVisualizationConfig(
            width,
            height,
            backgroundColor,
            actualCenter,
            rectangleColorProvider);
    }
}