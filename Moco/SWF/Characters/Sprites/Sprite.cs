using Moco.Rendering;
using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition;
using Moco.Timelining;

namespace Moco.SWF.Characters.Sprites;

/// <summary>
/// A sprite, which is pretty much its own small SWF file inside of the outer SWF file.
/// SWFception...
/// </summary>
public class Sprite : ICharacter
{
    /// <inheritdoc/>
    public int Id { get; init; }

    /// <summary>
    /// The timeline of the sprite.
    /// </summary>
    public Timeline Timeline { get; init; }

    /// <summary>
    /// Constructs a new sprite from the define sprite tag.
    /// </summary>
    /// <param name="defineSprite">The define sprite tag.</param>
    /// <param name="framerate">The framerate of the SWF file.</param>
    public Sprite(
        DefineSprite defineSprite,
        float framerate)
    {
        // NOTE(pref): I can't figure it out, does every sprite placed in the
        //             display list share the same timeline? Or does placing 
        //             a new sprite pretty much instantiate a new instance
        //             of said sprite.

        Id = defineSprite.CharacterId;
        Timeline = new(
            defineSprite.Tags!,
            framerate,
            int.MaxValue);
    }

    /// <summary>
    /// Ticks the sprite and its timeline.
    /// </summary>
    public void Tick()
    {
        Timeline.Tick();
    }

    /// <summary>
    /// Draws the sprite.
    /// </summary>
    /// <param name="ctx">The drawing context.</param>
    public void Draw(DisplayListDrawingContext ctx)
    {
        Timeline.Draw(ctx);
    }
}
