﻿using Moco.Rasterization;
using Moco.SWF.Characters;
using Moco.SWF.DataTypes;
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
    public SKBitmap Surface { get; init; }

    /// <summary>
    /// The render target.
    /// </summary>
    public GRBackendRenderTarget RenderTarget { get; init; }

    /// <summary>
    /// The shape's bounds.
    /// </summary>
    public Rectangle Bounds { get; init; }

    /// <summary>
    /// Constructs a new Skia backed shape.
    /// </summary>
    /// <param name="surface">The surface of the shape.</param>
    /// <param name="id">The id of the shape.</param>
    public SkiaMocoShape(
        SKBitmap surface, 
        GRBackendRenderTarget rt,
        Rectangle bounds,
        int id)
    {
        Surface = surface;
        RenderTarget = rt;
        Bounds = bounds;
        Id = id;
    }

    /// <inheritdoc/>
    public IMocoDrawingContext GetRasterizationContext()
    {
        return new SkiaMocoDrawingContext(Surface, Bounds);
    }
}
