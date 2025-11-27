using System.Drawing;

namespace TagCloud;

public class Spiral(Point center)
{
    private List<Rectangle> Rectangles { get; } = [];
    private double angle;

    public Rectangle AddRectangle(Size rectangleSize)
    {
        while (true)
        {
            var pointOnSpiral = GetNextPointOnSpiral();
            var candidate = CreateRectangleByPoint(pointOnSpiral, rectangleSize);

            if (IntersectsWithAny(candidate))
                continue;
            
            var compressed = MoveToCenter(candidate);
            Rectangles.Add(compressed);
            return compressed;
        }
    }

    public bool TryAddRectangleInCenter(Size rectangleSize, out Rectangle rectangle)
    {
        if (Rectangles.Count == 0)
        {
            var first = CreateRectangleByPoint(center, rectangleSize);
            Rectangles.Add(first);
            rectangle = first;
            return true;
        }
        rectangle = default;
        return false;
    }

    #region Helpers
    
    private static Rectangle CreateRectangleByPoint(Point point, Size size)
    {
        var x = point.X - size.Width / 2;
        var y = point.Y - size.Height / 2;
        
        return new Rectangle(x, y, size.Width, size.Height);
    }

    private bool IntersectsWithAny(Rectangle rect) =>
        Rectangles.Any(r => r.IntersectsWith(rect));

    private Rectangle MoveToCenter(Rectangle rect)
    {
        while (true)
        {
            var rectCenter = GetCenter(rect);
            var dx = Math.Sign(center.X - rectCenter.X);
            var dy = Math.Sign(center.Y - rectCenter.Y);
            
            if (dx == 0 && dy == 0)
                break;

            var shifted = rect with { X = rect.X + dx, Y = rect.Y + dy };
            
            if (IntersectsWithAny(shifted))
                break;

            rect = shifted;
        }

        return rect;
    }

    private static Point GetCenter(Rectangle rect) =>
        new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    
    private Point GetNextPointOnSpiral()
    {
        const double angleStep = 0.1; 
        const double radiusStep = 1.0;  

        angle += angleStep;
        var radius = radiusStep * angle;

        var x = center.X + (int)(radius * Math.Cos(angle));
        var y = center.Y + (int)(radius * Math.Sin(angle));

        return new Point(x, y);
    }
    
    #endregion
}