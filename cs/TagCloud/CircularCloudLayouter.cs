using System.Drawing;

namespace TagCloud;

public class CircularCloudLayouter(Point center)
{
    private Spiral RectangleSpiral { get; } = new(center);

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
            throw new ArgumentException("Размер прямоугольника должен быть положительным");
        
        return RectangleSpiral.TryAddRectangleInCenter(rectangleSize, out var rectangle)
            ? rectangle : RectangleSpiral.AddRectangle(rectangleSize);
    }
}
