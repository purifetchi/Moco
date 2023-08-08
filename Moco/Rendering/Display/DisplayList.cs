namespace Moco.Rendering.Display;

/// <summary>
/// The list of the characters that will be displayed in the next frame.
/// </summary>
public class DisplayList
{
    /// <summary>
    /// The entries of the display list.
    /// </summary>
    public IReadOnlyList<IDisplayListMember> Entries => _entries;

    /// <summary>
    /// The entries of the display list.
    /// </summary>
    private readonly List<IDisplayListMember> _entries;

    /// <summary>
    /// Constructs a new display list.
    /// </summary>
    public DisplayList()
    {
        _entries = new List<IDisplayListMember>();
    }

    /// <summary>
    /// Adds a tag to the display list.
    /// </summary>
    /// <param name="item">The tag.</param>
    public void Push(IDisplayListMember item)
    {
        _entries.Add(item);
        _entries.Sort((l, r) => r.Depth.CompareTo(l.Depth));
    }
}
