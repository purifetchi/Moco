using Moco.SWF.Characters;
using Moco.SWF.DataTypes;

namespace Moco.Rendering.Display;

/// <summary>
/// A display list object placed by PlaceObject.
/// </summary>
public class Object : IDisplayListMember
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

    /// <summary>
    /// Draws this object.
    /// </summary>
    /// <param name="ctx">The context.</param>
    public void Draw(DisplayListDrawingContext ctx)
    {
        var shape = (IShape)ctx.Engine.GetCharacter(CharacterId);
        ctx.Engine.Backend.PlaceShape(shape, Matrix);
    }
}
