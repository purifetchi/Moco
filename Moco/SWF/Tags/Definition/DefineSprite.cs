using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags.Control;

namespace Moco.SWF.Tags.Definition;

/// <summary>
/// Defines a sprite character. 
/// </summary>
public class DefineSprite : Tag,
    ICharacterDefinitionTag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.DefineSprite;
    
    /// <inheritdoc/>
    public override int MinimumVersion => 3;

    /// <summary>
    /// The frame count.
    /// </summary>
    public ushort FrameCount { get; private set; }

    /// <inheritdoc/>
    public ushort CharacterId { get; private set; }

    /// <summary>
    /// The tag list.
    /// </summary>
    public List<Tag>? Tags { get; private set; }

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        var br = reader.GetBinaryReader();
        CharacterId = br.ReadUInt16();
        FrameCount = br.ReadUInt16();

        Tags = new();

        Tag? tag;
        do
        {
            tag = reader.ReadTag();
            if (tag is null)
                continue;

            Tags.Add(tag);
        } while (tag is not End);

        return this;
    }
}
