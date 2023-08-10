using Moco.Rasterization;
using Moco.SWF.Characters;
using SkiaSharp;

namespace Moco.Skia.Backend;

/// <summary>
/// A skia implementation of the shape.
/// </summary>
public class SkiaMocoShape : IShape
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <summary>
    /// The backing surface.
    /// </summary>
    public SKSurface Surface { get; init; }

    /// <summary>
    /// Constructs a new Skia backed shape.
    /// </summary>
    /// <param name="surface">The surface of the shape.</param>
    /// <param name="id">The id of the shape.</param>
    public SkiaMocoShape(SKSurface surface, int id)
    {
        Surface = surface;
        Id = id;
    }

    /// <inheritdoc/>
    public IMocoDrawingContext GetRasterizationContext()
    {
        return new SkiaMocoDrawingContext(Surface.Canvas);
    }
}
