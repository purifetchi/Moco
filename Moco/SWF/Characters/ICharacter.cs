namespace Moco.SWF.Characters;

/// <summary>
/// A character that can be referenced by the swf.
/// <br />
/// This can be anything: sounds, images, etc.
/// </summary>
public interface ICharacter
{
    /// <summary>
    /// The id of said character.
    /// </summary>
    int Id { get; }
}
