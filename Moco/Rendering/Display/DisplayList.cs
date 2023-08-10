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

        // We need to sort the display list in the opposite direction, because of
        // the way we draw (first->last item)
        _entries.Sort((l, r) => l.Depth.CompareTo(r.Depth));
    }

    /// <summary>
    /// Clears this display list.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
    }

    /// <summary>
    /// Removes a member at a depth.
    /// </summary>
    /// <param name="depth">The depth.</param>
    public void RemoveAtDepth(int depth)
    {
        _entries.Remove(_entries.First(i => i.Depth == depth));
    }

    /// <summary>
    /// Gets the specified member at a depth.
    /// </summary>
    /// <param name="depth">The depth.</param>
    /// <returns>Said member, or nothing.</returns>
    public IDisplayListMember? GetAtDepth(int depth)
    {
        return _entries.FirstOrDefault(i => i.Depth == depth);
    }
}
