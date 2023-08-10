using Moco.Rasterization;

namespace Moco.SWF.Characters;

/// <summary>
/// A shape.
/// </summary>
public interface IShape : ICharacter
{
    /// <summary>
    /// Gets the rasterization context for this shape.
    /// </summary>
    /// <returns>The rasterization context.</returns>
    IMocoDrawingContext GetRasterizationContext();
}
