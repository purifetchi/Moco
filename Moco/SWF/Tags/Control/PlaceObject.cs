using Moco.Exceptions;
using Moco.Rendering.Display;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// The PlaceObject tag adds a character to the display list. 
/// </summary>
public class PlaceObject : Tag,
	IVersionedTag,
	IDisplayListAffectingTag
{
	/// <inheritdoc/>
	public override TagType Type => _actualType;

	/// <inheritdoc/>
	public override int MinimumVersion => _minimumVersion;

	/// <inheritdoc/>
	public int Version { get; init; }

	/// <summary>
	/// The place object flags.
	/// </summary>
	public PlaceObjectFlags Flags { get; private set; } = PlaceObjectFlags.None;

	/// <summary>
	/// The character id.
	/// </summary>
	public ushort CharacterId { get; private set; }

	/// <inheritdoc/>
	public int Depth { get; private set; }

	/// <summary>
	/// Transform matrix data.
	/// </summary>
	public Matrix Matrix { get; private set; } = Matrix.Identity;

	/// <summary>
	/// Color transform data.
	/// </summary>
	public ColorTransform ColorTransform { get; private set; }

	/// <summary>
	/// The ratio.
	/// </summary>
	public ushort Ratio { get; private set; }

	/// <summary>
	/// Name of the character.
	/// </summary>
	public string? Name { get; private set; }

	/// <summary>
	/// The clip depth.
	/// </summary>
	public ushort ClipDepth { get; private set; }

	/// <summary>
	/// The actual minimum version.
	/// </summary>
	private int _minimumVersion;

	/// <summary>
	/// The actual type.
	/// </summary>
	private TagType _actualType;

	/// <summary>
	/// Constructs a new place object with the given version.
	/// </summary>
	/// <param name="version">The version.</param>
	public PlaceObject(int version = 1)
	{
		switch (version)
		{
			case 1:
				throw new MocoTodoException("Support PlaceObject.");

			case 2:
				_actualType = TagType.PlaceObject2;
				_minimumVersion = 3;
				break;

			case 3:
				throw new MocoTodoException("Support PlaceObject3.");
		}

		Version = version;
	}

	/// <inheritdoc/>
	public void Execute(DisplayList displayList)
	{
		DisplayObject? displayObject = null;

		// If we have HasCharacter and we don't have Move, we need to
		// create a new display list member.
		if (Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
			!Flags.HasFlag(PlaceObjectFlags.Move))
		{
			displayObject = new DisplayObject
			{
				Depth = Depth,
				CharacterId = CharacterId
			};
			displayList.Push(displayObject);
		}

		// If we have HasCharacter and we have the move flag, we need to replace
		// the already existing character with a new one at a depth.
		if (Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
			Flags.HasFlag(PlaceObjectFlags.Move))
		{
			displayList.ReplaceAtDepth(Depth, CharacterId);
			displayObject = displayList.GetAtDepth(Depth) as DisplayObject;
		}

		// If we have don't have HasCharacter and we have Move, we need to modify
		// the display list member at a given depth.
		if (!Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
			Flags.HasFlag(PlaceObjectFlags.Move))
		{
			displayObject = displayList.GetAtDepth(Depth) as DisplayObject;
		}

		if (displayObject is not null)
		{
			if (Flags.HasFlag(PlaceObjectFlags.HasMatrix))
				displayObject.Matrix = Matrix;
		}
	}

	/// <inheritdoc/>
	internal override Tag Parse(SwfReader reader, RecordHeader header)
	{
		var binaryReader = reader.GetBinaryReader();
		Flags = (PlaceObjectFlags)binaryReader.ReadByte();
		Depth = binaryReader.ReadUInt16();

		if (Flags.HasFlag(PlaceObjectFlags.HasCharacter))
			CharacterId = binaryReader.ReadUInt16();

		if (Flags.HasFlag(PlaceObjectFlags.HasMatrix))
			Matrix = reader.ReadMatrixRecord();

		if (Flags.HasFlag(PlaceObjectFlags.HasCXForm))
			ColorTransform = reader.ReadCXFormWithAlphaRecord();

		if (Flags.HasFlag(PlaceObjectFlags.HasRatio))
			Ratio = binaryReader.ReadUInt16();

		if (Flags.HasFlag(PlaceObjectFlags.HasName))
			Name = reader.ReadZeroTerminatedString();

		if (Flags.HasFlag(PlaceObjectFlags.HasClipDepth))
			ClipDepth = binaryReader.ReadUInt16();

		if (Flags.HasFlag(PlaceObjectFlags.HasClipActions))
			throw new MocoTodoException("Read the clip actions field the place object tag.");

		return this;
	}
}
