using Moco.SWF.DataTypes;

namespace Moco.Rendering;

/// <summary>
/// A drawing context for when we're drawing display list objects.
/// </summary>
public readonly struct DisplayListDrawingContext
{
    /// <summary>
    /// The Moco engine.
    /// </summary>
    public MocoEngine Engine { get; init; }

    /// <summary>
    /// The base matrix we're drawing with.
    /// </summary>
    public Matrix BaseMatrix { get; init; }

    /// <summary>
    /// Constructs a new display list drawing context.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="mat">The base drawing matrix.</param>
    public DisplayListDrawingContext(MocoEngine engine, Matrix mat)
    {
        Engine = engine;
        BaseMatrix = mat;
    }
}
