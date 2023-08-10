namespace Moco.SWF.Tags.Control;

/// <summary>
/// Flags for PlaceObject2/3.
/// </summary>
[Flags]
public enum PlaceObjectFlags : byte
{
    None = 0,

    Move = 1 << 0,
    HasCharacter = 1 << 1,
    HasMatrix = 1 << 2,
    HasCXForm = 1 << 3,
    HasRatio = 1 << 4,
    HasName = 1 << 5,
    HasClipDepth = 1 << 6,
    HasClipActions = 1 << 7,
}
