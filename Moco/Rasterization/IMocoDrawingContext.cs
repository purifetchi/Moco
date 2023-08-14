using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.Rasterization;

/// <summary>
/// A context exposing the drawing while rasterizing shapes.
/// </summary>
public interface IMocoDrawingContext : IDisposable
{
    /// <summary>
    /// Sets the engine.
    /// </summary>
    /// <param name="engine">The engine</param>
    void SetEngine(MocoEngine engine);

    /// <summary>
    /// Sets the fill style.
    /// </summary>
    /// <param name="style">The fill style.</param>
    void SetFill(FillStyle style);

    /// <summary>
    /// Sets the stroke style.
    /// </summary>
    /// <param name="style">The stroke style.</param>
    void SetStroke(LineStyle style);

    /// <summary>
    /// Moves the pen to a position.
    /// </summary>
    /// <param name="point">The point.</param>
    void MoveTo(Point point);

    /// <summary>
    /// Draws a line to somewhere.
    /// </summary>
    /// <param name="point">The point to end at.</param>
    void LineTo(Point point);

    /// <summary>
    /// Draws a cubic bezier curve to somewhere.
    /// </summary>
    /// <param name="control">The midway control point.</param>
    /// <param name="anchor">The final anchor point.</param>
    void CubicTo(Point control, Point anchor);

    /// <summary>
    /// Flushes the points.
    /// </summary>
    void FlushPoints();
}
