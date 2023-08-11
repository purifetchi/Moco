using System.ComponentModel.DataAnnotations;
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
    /// The shape's bounds.
    /// </summary>
    private Rectangle _bounds;

    /// <summary>
    /// The last path.
    /// </summary>
    private SKPath? _lastPath;

    /// <summary>
    /// Constructs a new skia moco drawing context.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public SkiaMocoDrawingContext(
        SKSurface surface, 
        Rectangle bounds)
    {
        _bounds = bounds;
        _surface = surface;
        _canvas = surface.Canvas;
        _paint = new SKPaint();
        _canvas.Clear(SKColors.Transparent);

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
        
        _lastX = newX;
        _lastY = newY;
    }

    /// <inheritdoc/>
    public void CubicToRelative(Twip cX, Twip cY, Twip aX, Twip aY)
    {
        const int steps = 10;
        const float dt = 1f / steps;

        // The curved-edge record stores the edge as two X-Y deltas. The three points that define the
        // Quadratic Bezier are calculated like this:

        // 1. The first anchor point is the current drawing position.
        var anchor1X = _lastX;
        var anchor1Y = _lastY;

        // 2. The control point is the current drawing position + ControlDelta.
        var controlX = new Twip(anchor1X.Value + cX.Value);
        var controlY = new Twip(anchor1Y.Value + cY.Value);

        // 3. The last anchor point is the current drawing position + ControlDelta + AnchorDelta
        var anchor2X = new Twip(controlX.Value + aX.Value);
        var anchor2Y = new Twip(controlY.Value + aY.Value);

        var t = 0f;
        for (var i = 0; i < steps; i++)
        {
            var oneMinusT = 1 - t;
            var dX = oneMinusT * (oneMinusT * anchor1X.LogicalPixelValue + t * controlX.LogicalPixelValue)
                + t * (oneMinusT * controlX.LogicalPixelValue + t * anchor2X.LogicalPixelValue);

            var dY = oneMinusT * (oneMinusT * anchor1Y.LogicalPixelValue + t * controlY.LogicalPixelValue)
                + t * (oneMinusT * controlY.LogicalPixelValue + t * anchor2Y.LogicalPixelValue);

            _points.Add(new SKPoint(dX, dY));

            t += dt;
        }

        _points.Add(new SKPoint(anchor2X.LogicalPixelValue, anchor2Y.LogicalPixelValue));

        _lastX = anchor2X;
        _lastY = anchor2Y;
    }

    /// <inheritdoc/>
    public void MoveTo(Twip x, Twip y)
    {
        if (_points.Count < 2)
            _points.Clear();
        else if (_points.Count >= 2)
            FlushPoints();

        _lastX = new Twip(x.Value/* - _bounds.XMin.Value*/);
        _lastY = new Twip(y.Value/* - _bounds.YMin.Value*/);

        _points.Add(new SKPoint(_lastX.LogicalPixelValue, _lastY.LogicalPixelValue));
    }

    /// <inheritdoc/>
    public void SetFill(FillStyle style)
    {
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

        var path = new SKPath();
        path.AddPoly(_points.ToArray(), true);
        _canvas.DrawPath(path, _paint);

        if (_lastPath != null)
        {
            using var intersect = path.Op(_lastPath, SKPathOp.Intersect);

            // TODO(pref): This should be filled with the FillStyle1 fill style.
            using (new SKAutoCanvasRestore(_canvas))
            {
                _canvas.ClipPath(intersect, antialias: true);
                _canvas.DrawColor(SKColors.Transparent);
            }
        }

        _canvas.Flush();

        // XOR the paths together, basically creating a hole where the old and the new path
        // intersect.
        _lastPath = _lastPath == null ? 
            path : 
            _lastPath!.Op(path, SKPathOp.Xor);

        _points.Clear();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        // TODO(pref): I've tried various things but the canvas will not draw
        //             at all without this thing.
        using var _ = _surface.Snapshot();

        _paint.Dispose();
        _canvas.Dispose();
    }
}
