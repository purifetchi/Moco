using Moco.SWF.Characters;
using Moco.SWF.Characters.Sprites;
using Moco.SWF.DataTypes;

namespace Moco.Rendering.Display;

/// <summary>
/// A display list object placed by PlaceObject.
/// </summary>
public class DisplayObject : IDisplayListMember
{
    /// <summary>
    /// The character id.
    /// </summary>
    public ushort CharacterId { get; set; }

    /// <summary>
    /// The depth.
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    /// The object's matrix.
    /// </summary>
    public Matrix Matrix { get; set; }

    /// <inheritdoc/>
    public void Draw(DisplayListDrawingContext ctx)
    {
        var maybeObject = ctx.Engine.GetCharacter(CharacterId);

        if (maybeObject is IShape shape)
        {
            ctx.Engine.Backend.PlaceShape(shape, ctx.BaseMatrix.Combine(Matrix));
        }
        else if (maybeObject is Sprite sprite)
        {
            sprite.Tick();
            sprite.Draw(new DisplayListDrawingContext(ctx.Engine, ctx.BaseMatrix.Combine(Matrix)));
        }
        else
        {
            Console.WriteLine($"[Object::Draw] Missing shape at id {CharacterId} (is it an object we can't read yet?)");
            return;
        }
    }

    /// <inheritdoc/>
    public void Replace(ushort characterId)
    {
        CharacterId = characterId;
    }
}
