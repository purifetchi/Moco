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
    /// Constructs a new skia moco drawing context.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public SkiaMocoDrawingContext(SKCanvas canvas)
    {
        _canvas = canvas;
        _paint = new SKPaint();
        _points = new();
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
        _paint.Color = SKColors.Red;
        switch (style.Type) 
        {
            case FillStyleType.ClippedBitmap:
                Console.WriteLine("[SetFill] Clipped bitmap");
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

        _canvas.DrawPoints(SKPointMode.Polygon,
            _points.ToArray(),
            _paint);

        _points.Clear();
    }
}
