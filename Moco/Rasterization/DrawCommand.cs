using System.Diagnostics.CodeAnalysis;
using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.Rasterization;

/// <summary>
/// A draw command.
/// </summary>
internal class DrawCommand : IEquatable<DrawCommand>
{
    /// <summary>
    /// Is it a straight line?
    /// </summary>
    public bool IsStraight { get; set; }

    /// <summary>
    /// The beginning point
    /// </summary>
    public Point From { get; set; }

    /// <summary>
    /// The control point.
    /// </summary>
    public Point? Control { get; set; }

    /// <summary>
    /// The final point.
    /// </summary>
    public Point To { get; set; }

    /// <summary>
    /// The fill style index.
    /// </summary>
    public int? FillStyleIdx { get; set; }

    /// <summary>
    /// The line style index.
    /// </summary>
    public int? LineStyleIdx { get; set; }

    /// <summary>
    /// Reverses this draw command and sets the new fill style.
    /// </summary>
    /// <param name="newFillStyle">The new fill style.</param>
    /// <returns>The new draw command.</returns>
    public DrawCommand Reverse(int? newFillStyle)
    {
        return new DrawCommand
        {
            IsStraight = IsStraight,
            From = To,
            Control = Control,
            To = From,
            FillStyleIdx = newFillStyle,
            LineStyleIdx = LineStyleIdx
        };
    }

    /// <summary>
    /// Checks if two draw commands are the same.
    /// </summary>
    /// <param name="left">The left draw command.</param>
    /// <param name="right">The right draw command.</param>
    /// <returns>Whether they're the same.</returns>
    public static bool operator==(DrawCommand left, DrawCommand right)
    {
        return left.IsStraight == right.IsStraight &&
            left.From == right.From &&
            left.Control == right.Control &&
            left.To == right.To &&
            left.FillStyleIdx == right.FillStyleIdx &&
            left.LineStyleIdx == right.LineStyleIdx;
    }

    /// <summary>
    /// Checks if two draw commands aren't the same.
    /// </summary>
    /// <param name="left">The left draw command.</param>
    /// <param name="right">The right draw command.</param>
    /// <returns>Whether they aren't the same.</returns>
    public static bool operator!=(DrawCommand left, DrawCommand right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is DrawCommand cmd &&
            this == cmd;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(IsStraight, From, Control, To, FillStyleIdx, LineStyleIdx);
    }

    /// <inheritdoc/>
    public bool Equals(DrawCommand? other)
    {
        return other is not null && 
            this == other;
    }
}
