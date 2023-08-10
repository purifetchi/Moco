using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags;

namespace Moco.SWF;

/// <summary>
/// The swf file.
/// </summary>
public class Swf
{
    /// <summary>
    /// The tag list.
    /// </summary>
    public IReadOnlyList<Tag> Tags => _tags;

    /// <summary>
    /// The frame size.
    /// </summary>
    public Rectangle FrameSize { get; init; }

    /// <summary>
    /// The frame count.
    /// </summary>
    public int FrameCount { get; init; }

    /// <summary>
    /// The framerate.
    /// </summary>
    public float FrameRate { get; init; }

    /// <summary>
    /// The swf version.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// The tag list.
    /// </summary>
    private readonly List<Tag> _tags;

    /// <summary>
    /// Constructs a new swf with a header.
    /// </summary>
    /// <param name="header">The header.</param>
    internal Swf(SwfHeader header)
    {
        _tags = new List<Tag>();

        FrameSize = header.FrameSize;
        FrameRate = header.FrameRate;
        FrameCount = header.FrameCount;
        Version = header.Version;
    }
    
    /// <summary>
    /// Tries to get a tag from within the swf.
    /// </summary>
    /// <typeparam name="TTag">The tag.</typeparam>
    /// <returns>The tag, or nothing.</returns>
    public TTag? GetTag<TTag>()
        where TTag : Tag
    {
        var tag = _tags.FirstOrDefault(tag => tag is TTag) as TTag;
        return tag;
    }

    /// <summary>
    /// Adds a tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    internal void AddTag(Tag tag)
    {
        if (tag == null)
            return;

        if (Version < tag.MinimumVersion)
        {
            Console.WriteLine($"Discarding tag {tag.Type} due to the swf version being too low.");
            return;
        }

        _tags.Add(tag);
    }
}
