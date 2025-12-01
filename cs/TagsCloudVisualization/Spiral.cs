using System.Drawing;

namespace TagCloud;

public class Spiral(Point center, double angleStep = 0.01, double radiusСoeff = 1.0)
{
    private double angle;

    public Point GetNextPoint(int steps = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(steps);

        angle += angleStep * steps;
        var radius = radiusСoeff * angle;

        var x = center.X + (int)Math.Round(radius * Math.Cos(angle));
        var y = center.Y + (int)Math.Round(radius * Math.Sin(angle));

        return new Point(x, y);
    }
}
