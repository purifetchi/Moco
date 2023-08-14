using Moco.Exceptions;
using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;
using Moco.SWF.Tags.Definition.Shapes.Records;

namespace Moco.Rasterization;

/// <summary>
/// The class responsible for reading the shape records and building discrete paths out of them.
/// <br />
/// This code was partially adapted from as3swf and the corresponding blog post, thank you!
/// </summary>
internal class PathBuilder
{
    /// <summary>
    /// The shape with style.
    /// </summary>
    private readonly ShapeWithStyle _shape;

    /// <summary>
    /// The figures.
    /// </summary>
    private readonly List<Figure> _figures;

    /// <summary>
    /// The current figure.
    /// </summary>
    private Figure _currentFigure;

    /// <summary>
    /// Creates a new path builder for a given shape with style.
    /// </summary>
    /// <param name="shape">The shape.</param>
    public PathBuilder(ShapeWithStyle shape)
    {
        _shape = shape;

        _figures = new();
        _currentFigure = new(shape.FillStyles, shape.LineStyles);
    }

    /// <summary>
    /// Creates edge maps for this shape.
    /// </summary>
    public PathBuilder CreateEdgeMaps()
    {
        const StyleChangeRecordFlags anyStyleChangedMask = StyleChangeRecordFlags.HasFillStyle0 | 
            StyleChangeRecordFlags.HasFillStyle1 | 
            StyleChangeRecordFlags.HasLineStyle;

        var pos = new Point(Twip.Zero, Twip.Zero);
        var subPath = new EdgePath();

        var stylesCtx = new ShapeStylesContext();

        foreach (var record in _shape.ShapeRecords)
        {
            switch (record)
            {
                case StyleChangeRecord styleChangeRecord:
                    // Check if we've changed any of the styles.
                    if ((styleChangeRecord.Flags | anyStyleChangedMask) != 0)
                    {
                        _currentFigure.AddPath(subPath, stylesCtx);
                        subPath = new EdgePath();
                    }            

                    // Check if all of the styles were reset. This should signify the start
                    // of a new group.
                    if (styleChangeRecord.Flags.HasFlag(anyStyleChangedMask) &&
                        styleChangeRecord.FillStyle0 == 0 &&
                        styleChangeRecord.FillStyle1 == 0 &&
                        styleChangeRecord.LineStyle == 0)
                    {
                        _currentFigure.Clean();
                        _figures.Add(_currentFigure);

                        _currentFigure = new(_currentFigure.FillStyles, _currentFigure.LineStyles);
                        
                        // A new group should also have new styles set alongside it.
                        if (styleChangeRecord.Flags.HasFlag(StyleChangeRecordFlags.HasNewStyles))
                        {
                            _currentFigure.FillStyles = styleChangeRecord.FillStyles!;
                            _currentFigure.LineStyles = styleChangeRecord.LineStyles!;
                        }
                        else
                        {
                            throw new MocoTodoException("A new group has no styles associated with it?");
                        }

                        stylesCtx = new();
                    }
                    else
                    {
                        // In this section we basically set the index of each style
                        // And *if* the value is more than 0, we adjust it via the offset into the
                        // combined style list.

                        if (styleChangeRecord.Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle0))
                        {
                            if (styleChangeRecord.FillStyle0 > 0)
                                stylesCtx.FillStyle0 = (int)(styleChangeRecord.FillStyle0 - 1);
                            else
                                stylesCtx.FillStyle0 = null;
                        }

                        if (styleChangeRecord.Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle1))
                        {
                            if (styleChangeRecord.FillStyle1 > 0)
                                stylesCtx.FillStyle1 = (int)(styleChangeRecord.FillStyle1 - 1);
                            else
                                stylesCtx.FillStyle1 = null;
                        }

                        if (styleChangeRecord.Flags.HasFlag(StyleChangeRecordFlags.HasLineStyle))
                        {
                            if (styleChangeRecord.LineStyle > 0)
                                stylesCtx.LineStyle = (int)(styleChangeRecord.LineStyle - 1);
                            else
                                stylesCtx.LineStyle = null;
                        }
                    }

                    // Set the position of the move to.
                    if (styleChangeRecord.Flags.HasFlag(StyleChangeRecordFlags.HasMoveTo))
                        pos = new Point(styleChangeRecord.MoveDeltaX, styleChangeRecord.MoveDeltaY);
                    break;

                case StraightEdgeRecord straightEdgeRecord:
                    {
                        var from = pos;
                        var to = from + new Point(straightEdgeRecord.DeltaX, straightEdgeRecord.DeltaY);

                        pos = to;

                        subPath.AddCommand(new DrawCommand
                        {
                            IsStraight = true,
                            From = from,
                            To = to,

                            FillStyleIdx = stylesCtx.FillStyle1,
                            LineStyleIdx = stylesCtx.LineStyle
                        });
                        break;
                    }

                case CurvedEdgeRecord curvedEdgeRecord:
                    {
                        var from = pos;
                        var control = from + new Point(curvedEdgeRecord.ControlDeltaX, curvedEdgeRecord.ControlDeltaY);
                        var to = control + new Point(curvedEdgeRecord.AnchorDeltaX, curvedEdgeRecord.AnchorDeltaY);

                        pos = to;

                        subPath.AddCommand(new DrawCommand
                        {
                            IsStraight = false,
                            From = from,
                            Control = control,
                            To = to,

                            FillStyleIdx = stylesCtx.FillStyle1,
                            LineStyleIdx = stylesCtx.LineStyle
                        });
                        break;
                    }

                case EndShapeRecord:
                    // Process the final subpath.
                    _currentFigure.AddPath(subPath, stylesCtx);
                    _currentFigure.Clean();

                    _figures.Add(_currentFigure);
                    break;
            }
        }

        return this;
    }

    /// <summary>
    /// Rasterizes this shape into a Moco drawing context.
    /// </summary>
    /// <param name="ctx">The drawing context.</param>
    public void Rasterize(IMocoDrawingContext ctx)
    {
        foreach (var figure in _figures)
        {
            RasterizeFill(ctx, figure);
            RasterizeStroke(ctx, figure);
        }
    }

    /// <summary>
    /// Rasterizes the fill of a figure.
    /// </summary>
    /// <param name="ctx">The drawing context.</param>
    /// <param name="figure">The figure.</param>
    private void RasterizeFill(
        IMocoDrawingContext ctx,
        Figure figure)
    {
        var path = figure.FlattenFills();
        if (path.Commands.Count < 1)
            return;

        var pos = new Point(new Twip(int.MaxValue), new Twip(int.MaxValue));
        int? lastFillIdx = int.MaxValue;

        foreach (var edge in path.Commands)
        {
            if (lastFillIdx != edge.FillStyleIdx)
            {
                if (lastFillIdx.HasValue)
                    ctx.FlushPoints();

                lastFillIdx = edge.FillStyleIdx;
                if (!lastFillIdx.HasValue)
                    ctx.SetFill(null!);
                else
                    ctx.SetFill(figure.FillStyles[lastFillIdx.Value]);

                pos = new Point(new Twip(int.MaxValue), new Twip(int.MaxValue));
            }

            if (pos != edge.From)
                ctx.MoveTo(edge.From);

            if (edge.IsStraight)
                ctx.LineTo(edge.To);
            else
                ctx.CubicTo(edge.Control!.Value, edge.To);

            pos = edge.To;
        }

        ctx.FlushPoints();
    }

    /// <summary>
    /// Rasterizes the stroke of a figure.
    /// </summary>
    /// <param name="ctx">The context.</param>
    /// <param name="figure">The figure.</param>
    private void RasterizeStroke(
        IMocoDrawingContext ctx,
        Figure figure)
    {
        var path = figure.FlattenLines();
        if (path.Commands.Count < 1)
            return;

        var pos = new Point(new Twip(int.MaxValue), new Twip(int.MaxValue));
        int? lastLineIdx = int.MaxValue;

        foreach (var edge in path.Commands)
        {
            if (lastLineIdx != edge.LineStyleIdx)
            {
                if (lastLineIdx.HasValue)
                    ctx.FlushPoints();

                lastLineIdx = edge.LineStyleIdx;
                if (!lastLineIdx.HasValue)
                    ctx.SetStroke(null!);
                else
                    ctx.SetStroke(figure.LineStyles[lastLineIdx.Value]);

                pos = new Point(new Twip(int.MaxValue), new Twip(int.MaxValue));
            }

            if (pos != edge.From)
                ctx.MoveTo(edge.From);

            if (edge.IsStraight)
                ctx.LineTo(edge.To);
            else
                ctx.CubicTo(edge.Control!.Value, edge.To);

            pos = edge.To;
        }

        ctx.FlushPoints();
    }
}
