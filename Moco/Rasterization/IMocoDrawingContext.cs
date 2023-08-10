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
    /// Moves the pen to a position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    void MoveTo(Twip x, Twip y);

    /// <summary>
    /// Draws a (relative positioned) line to somewhere.
    /// </summary>
    /// <param name="x">The relative x coordinate value.</param>
    /// <param name="y">The relative y coordinate value.</param>
    void LineToRelative(Twip x, Twip y);

    /// <summary>
    /// Draws a (relative positioned) cubic bezier curve to somewhere.
    /// </summary>
    /// <param name="cX">The x control point.</param>
    /// <param name="cY">The y control point.</param>
    /// <param name="aX">The x anchor point.</param>
    /// <param name="aY">The y anchor point.</param>
    void CubicToRelative(Twip cX, Twip cY, Twip aX, Twip aY);

    /// <summary>
    /// Flushes the points.
    /// </summary>
    void FlushPoints();
}
