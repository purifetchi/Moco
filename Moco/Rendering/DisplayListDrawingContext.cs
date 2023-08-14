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
    /// Constructs a new display list drawing context.
    /// </summary>
    /// <param name="engine">The engine.</param>
    public DisplayListDrawingContext(MocoEngine engine)
    {
        Engine = engine;
    }
}
