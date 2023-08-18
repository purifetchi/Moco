using Moco.Rasterization;
using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;
using SkiaSharp;

namespace Moco.Skia.Backend;

/// <summary>
/// A moco drawing context utilizing Skia for rasterization.
/// </summary>
public class SkiaMocoDrawingContext : IMocoDrawingContext
{
    /// <summary>
    /// The canvas we're drawing to.
    /// </summary>
    private SKCanvas _canvas;

    /// <summary>
    /// The paint.
    /// </summary>
    private SKPaint _paint;

    /// <summary>
    /// The last point.
    /// </summary>
    private Point _lastPoint;

    /// <summary>
    /// The Moco engine.
    /// </summary>
    private MocoEngine? _engine;

    /// <summary>
    /// The shape's bounds.
    /// </summary>
    private Rectangle _bounds;

    /// <summary>
    /// The last path.
    /// </summary>
    private SKPath _path;

    /// <summary>
    /// Constructs a new skia moco drawing context.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public SkiaMocoDrawingContext(
        SKBitmap surface, 
        Rectangle bounds)
    {
        _bounds = bounds;
        _canvas = new SKCanvas(surface);
        _paint = new SKPaint();
        _canvas.Clear(SKColors.Transparent);

        _lastPoint = new(
            new Twip(-bounds.XMin.Value), 
            new Twip(-bounds.YMin.Value)
            );

        _path = new();
    }

    /// <inheritdoc/>
    public void SetEngine(MocoEngine engine)
    {
        _engine = engine;
    }

    /// <inheritdoc/>
    public void LineTo(Point point)
    {
        var offsetPoint = new Point(
            new Twip(point.X.Value - _bounds.XMin.Value),
            new Twip(point.Y.Value - _bounds.YMin.Value));

        _path.LineTo(
            offsetPoint.X.LogicalPixelValue,
            offsetPoint.Y.LogicalPixelValue);

        _lastPoint = offsetPoint;
    }

    /// <inheritdoc/>
    public void CubicTo(Point control, Point anchor)
    {
        var offsetControl = new Point(
            new Twip(control.X.Value - _bounds.XMin.Value),
            new Twip(control.Y.Value - _bounds.YMin.Value));

        var offsetAnchor = new Point(
            new Twip(anchor.X.Value - _bounds.XMin.Value),
            new Twip(anchor.Y.Value - _bounds.YMin.Value));

        _path.CubicTo(_lastPoint.X.LogicalPixelValue, _lastPoint.Y.LogicalPixelValue,
            offsetControl.X.LogicalPixelValue, offsetControl.Y.LogicalPixelValue,
            offsetAnchor.X.LogicalPixelValue, offsetAnchor.Y.LogicalPixelValue);

        _lastPoint = offsetAnchor;
    }

    /// <inheritdoc/>
    public void MoveTo(Point point)
    {
        _lastPoint = new Point(
            new Twip(point.X.Value - _bounds.XMin.Value),
            new Twip(point.Y.Value - _bounds.YMin.Value));

        _path.MoveTo(
            _lastPoint.X.LogicalPixelValue, 
            _lastPoint.Y.LogicalPixelValue);
    }

    /// <inheritdoc/>
    public void SetFill(FillStyle style)
    {
        _paint.Dispose();

        _paint = new SKPaint();
        _paint.Style = SKPaintStyle.Fill;
        _paint.IsAntialias = true;

        if (style is null)
        {
            _paint.Color = SKColors.Transparent;
            return;
        }

        switch (style.Type) 
        {
            case FillStyleType.Solid:
                _paint.Color = new SKColor(
                    style.Color.Red, 
                    style.Color.Green, 
                    style.Color.Blue, 
                    style.Color.Alpha);
                break;

            case FillStyleType.ClippedBitmap:
            case FillStyleType.NonSmoothedClippedBitmap:
                var bitmap = (SKBitmap)_engine!.GetCharacter(style.BitmapId)!;
                if (bitmap is null)
                {
                    Console.WriteLine($"No bitmap when setting fill for character... Are we missing some image implementation?");
                    _paint.Color = SKColors.Pink;
                    return;
                }
                _paint.Shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
                break;

            default:
                throw new Exception("[SkiaMocoDrawingContext] Support more fill styles.");
        }
    }

    /// <inheritdoc/>
    public void SetStroke(LineStyle style)
    {
        if (style is LineStyle2)
            Console.WriteLine($"[SkiaMocoDrawingContext::SetStroke] This is a LineStyle2 stroke, we will not draw it properly yet.");

        _paint?.Dispose();

        if (style is null)
        {
            _paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Transparent
            };
            return;
        }

        _paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            Color = new SKColor(style.Color.Red, style.Color.Green, style.Color.Blue, style.Color.Alpha),
            StrokeWidth = new Twip(style.Width).LogicalPixelValue,
            StrokeJoin = SKStrokeJoin.Round,
            StrokeCap = SKStrokeCap.Round
        };
    }

    /// <inheritdoc/>
    public void FlushPoints()
    {
        _path.Close();

        _canvas.DrawPath(_path, _paint);
        _path.Reset();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _path.Dispose();
        _paint.Dispose();
        _canvas.Dispose();
    }
}
