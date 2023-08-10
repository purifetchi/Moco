using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.Rasterization;

/// <summary>
/// A context exposing the drawing while rasterizing shapes.
/// </summary>
public interface IMocoDrawingContext
{
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
    /// Flushes the points.
    /// </summary>
    void FlushPoints();
}
