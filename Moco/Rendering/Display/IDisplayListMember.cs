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
}
