namespace Moco.SWF.Tags;

/// <summary>
/// An interface for a versioned tag.
/// </summary>
public interface IVersionedTag
{
    /// <summary>
    /// The version of the tag.
    /// </summary>
    public int Version { get; }
}
