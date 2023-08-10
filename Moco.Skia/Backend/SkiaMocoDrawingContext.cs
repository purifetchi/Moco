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
    /// The surface we're drawing on.
    /// </summary>
    private SKSurface _surface;

    /// <summary>
    /// The paint.
    /// </summary>
    private SKPaint _paint;

    /// <summary>
    /// The last x position.
    /// </summary>
    private Twip _lastX = Twip.Zero;

    /// <summary>
    /// The last y position.
    /// </summary>
    private Twip _lastY = Twip.Zero;

    /// <summary>
    /// The list of points.
    /// </summary>
    private List<SKPoint> _points;

    /// <summary>
    /// The Moco engine.
    /// </summary>
    private MocoEngine? _engine;

    /// <summary>
    /// Constructs a new skia moco drawing context.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public SkiaMocoDrawingContext(SKSurface surface)
    {
        _surface = surface;
        _canvas = surface.Canvas;
        _paint = new SKPaint();

        _points = new()
        {
            new SKPoint(0, 0)
        };
    }

    /// <inheritdoc/>
    public void SetEngine(MocoEngine engine)
    {
        _engine = engine;
    }

    /// <inheritdoc/>
    public void LineToRelative(Twip x, Twip y)
    {
        var newX = new Twip(_lastX.Value + x.Value);
        var newY = new Twip(_lastY.Value + y.Value);

        _points.Add(new SKPoint(newX.LogicalPixelValue, newY.LogicalPixelValue));

        Console.WriteLine($"[LineToRelative] Line from {_lastX.LogicalPixelValue}x{_lastY.LogicalPixelValue} to {newX.LogicalPixelValue}x{newY.LogicalPixelValue}");
        
        _lastX = newX;
        _lastY = newY;
    }

    /// <inheritdoc/>
    public void MoveTo(Twip x, Twip y)
    {
        Console.WriteLine($"[MoveTo] Moving to: {x.LogicalPixelValue}x{y.LogicalPixelValue}");
        _lastX = x;
        _lastY = y;

        _points.Add(new SKPoint(x.LogicalPixelValue, y.LogicalPixelValue));
    }

    /// <inheritdoc/>
    public void SetFill(FillStyle style)
    {
        _paint = new SKPaint();
        _paint.Style = SKPaintStyle.Fill;
        if (style is null)
            return;

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
                var bitmap = (SKBitmap)_engine!.GetCharacter(style.BitmapId);
                _paint.Shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
                break;

            default:
                throw new Exception("[SkiaMocoDrawingContext] Support more fill styles.");
        }
    }

    /// <inheritdoc/>
    public void FlushPoints()
    {
        if (_points.Count < 1)
            return;

        using var path = new SKPath();
        path.AddPoly(_points.ToArray());
        _canvas.DrawPath(path, _paint);
        _canvas.Flush();

        _points.Clear();
        
        // TODO(pref): I've tried various things but the canvas will not draw
        //             at all without this thing.
        using var _ = _surface.Snapshot();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _paint.Dispose();
        _canvas.Dispose();
    }
}
