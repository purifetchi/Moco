namespace Moco.Rendering.Display;

/// <summary>
/// Inherited by any tag that's part of the display list.
/// </summary>
public interface IDisplayListMember
{
    /// <summary>
    /// The z-depth of the display list item.
    /// </summary>
    int Depth { get; }

    /// <summary>
    /// Draws this object.
    /// </summary>
    /// <param name="ctx">The drawing context.</param>
    void Draw(DisplayListDrawingContext ctx);

    /// <summary>
    /// Replace the object at this depth with a different character.
    /// </summary>
    /// <param name="characterId">The character id.</param>
    void Replace(ushort characterId);
}
