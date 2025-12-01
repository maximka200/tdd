using System.Drawing;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace TagCloud;

public class CircularCloudLayouter(Point center)
{
    private readonly Spiral spiral = new(center);
    private readonly List<Rectangle> rectangles = [];
    public IReadOnlyCollection<Rectangle> Rectangles => rectangles.AsReadOnly();
    
    private int failureStreak;
    private int currentStep = DefaultStep;

    private const int DefaultStep = 1;
    private const int MaxStep = 1000;

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Width <= 0)
            throw new ArgumentOutOfRangeException(nameof(rectangleSize.Width),
                "Ширина прямоугольника должна быть положительной");

        if (rectangleSize.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(rectangleSize.Height),
                "Высота прямоугольника должна быть положительной");
        
        if (rectangles.Count == 0)
        {
            var first = CreateRectangleByCenter(center, rectangleSize);
            rectangles.Add(first);
            ResetStepStrategy();
            return first;
        }

        while (true)
        {
            var pointOnSpiral = spiral.GetNextPoint(currentStep);
            var candidate = CreateRectangleByCenter(pointOnSpiral, rectangleSize);

            if (IntersectsWithAny(candidate))
            {
                RegisterFailure();
                continue;
            }
            
            var compressed = MoveToCenter(candidate);
            rectangles.Add(compressed);
            ResetStepStrategy();
            return compressed;
        }
    }
    
    private void ResetStepStrategy()
    {
        failureStreak = 0;
        currentStep = DefaultStep;
    }

    private void RegisterFailure()
    {
        failureStreak++;
        
        currentStep = Math.Min(1 << Math.Min(failureStreak, 10), MaxStep);

        if (currentStep > MaxStep)
            currentStep = MaxStep;
    }

    private static Rectangle CreateRectangleByCenter(Point point, Size size)
    {
        var x = point.X - size.Width / 2;
        var y = point.Y - size.Height / 2;
        return new Rectangle(x, y, size.Width, size.Height);
    }

    private bool IntersectsWithAny(Rectangle rect) =>
        rectangles.Any(r => r.IntersectsWith(rect));
    
    private Rectangle MoveToCenter(Rectangle rectangle)
    {
        while (true)
        {
            var rectCenter = GetCenter(rectangle);
            var dx = Math.Sign(center.X - rectCenter.X);
            var dy = Math.Sign(center.Y - rectCenter.Y);
            
            if (dx == 0 && dy == 0)
                return rectangle;

            var before = rectangle;
            
            var rough = MoveAtTheWay(rectangle, dx, dy, toCenter: true, maxStep: 64);
            
            var exact = MoveAtTheWay(rough, dx, dy, toCenter: true, maxStep: 1);

            rectangle = exact;
            
            if (rectangle == before)
                return rectangle;
        }
    }

    private Rectangle MoveAtTheWay(Rectangle rectangle, int dx, int dy, bool toCenter, int maxStep)
    {
        var step = 1;
        var sign = toCenter ? 1 : -1;

        while (step <= maxStep)
        {
            var shifted = rectangle with
            {
                X = rectangle.X + sign * dx * step,
                Y = rectangle.Y + sign * dy * step
            };

            if (IntersectsWithAny(shifted))
                break;

            rectangle = shifted;
            step *= 2;
        }

        return rectangle;
    }

    private static Point GetCenter(Rectangle rect) =>
        new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
}
