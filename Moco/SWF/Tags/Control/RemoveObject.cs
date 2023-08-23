using Moco.Exceptions;
using Moco.Rendering.Display;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;
using Moco.Timelining;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// The RemoveObject tag removes the character at the specified depth from the display list.
/// </summary>
public class RemoveObject : Tag,
    IVersionedTag,
	IDisplayListAffectingTag
{
    /// <inheritdoc/>
    public override TagType Type => _actualTag;

    /// <inheritdoc/>
    public override int MinimumVersion => _actualMinimumVersion;

    /// <inheritdoc/>
    public int Version { get; init; }

    /// <summary>
    /// ID of character to remove.
    /// </summary>
    public int? CharacterId { get; private set; }

    /// <summary>
    /// Depth of character.
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    /// The actual minimum version of the tag.
    /// </summary>
    private readonly int _actualMinimumVersion;

    /// <summary>
    /// The actual tag type.
    /// </summary>
    private readonly TagType _actualTag;

    /// <summary>
    /// Creates a new remove object tag with a given version.
    /// </summary>
    /// <param name="version">The version.</param>
    public RemoveObject(int version = 1)
    {
        switch (version)
        {
            case 1:
                _actualMinimumVersion = 1;
                _actualTag = TagType.RemoveObject;
                break;

            case 2:
                _actualMinimumVersion = 3;
                _actualTag = TagType.RemoveObject2;
                break;

            default:
                throw new NotSupportedException("There are no other tags than RemoveObject and RemoveObject2.");
        }
    }

    /// <inheritdoc/>
    public void Execute(DisplayList displayList)
    {
        // RemoveObject1 specific.
        if (CharacterId.HasValue)
        {
            var objectAtDepth = displayList.GetAtDepth(Depth);
            if (objectAtDepth is DisplayObject displayObject &&
                displayObject.CharacterId != CharacterId.Value)
            {
                Console.WriteLine("Mismatch when doing RemoveObject.");
                return;
            }
        }

		displayList.RemoveAtDepth(Depth);
	}

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        var br = reader.GetBinaryReader();
        if (Version == 1)
            CharacterId = br.ReadUInt16();

        Depth = br.ReadUInt16();

        return this;
    }
}
