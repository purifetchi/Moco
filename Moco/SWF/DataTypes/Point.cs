namespace Moco.SWF.DataTypes;

/// <summary>
/// An XY twip pair.
/// </summary>
public readonly struct Point : IEquatable<Point>
{
    /// <summary>
    /// The X position.
    /// </summary>
    public Twip X { get; init; }

    /// <summary>
    /// The Y position.
    /// </summary>
    public Twip Y { get; init; }

    /// <summary>
    /// Constructs a new point from the XY values.
    /// </summary>
    /// <param name="x">The X value.</param>
    /// <param name="y">The Y value.</param>
    public Point(Twip x, Twip y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Adds a point to another point.
    /// </summary>
    /// <param name="a">The 1st point.</param>
    /// <param name="b">The 2nd point.</param>
    /// <returns>The resulting point.</returns>
    public static Point operator+(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }
    
    /// <summary>
    /// Checks whether two points match.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>Whether they match.</returns>
    public static bool operator==(Point a, Point b)
    {
        return a.X.Value == b.X.Value &&
            a.Y.Value == b.Y.Value;
    }

    /// <summary>
    /// Checks whether two points don't match.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>Whether they don't match.</returns>
    public static bool operator !=(Point a, Point b)
    {
        return !(a == b);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Point point &&
            this == point;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(X.Value, Y.Value);
    }

    /// <inheritdoc/>
    public bool Equals(Point other)
    {
        return this == other;
    }
}
