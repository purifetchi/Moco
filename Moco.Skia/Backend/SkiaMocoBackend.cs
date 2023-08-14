using System.Runtime.InteropServices;
using Moco.Rendering;
using Moco.SWF.Characters;
using Moco.SWF.DataTypes;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using SkiaSharp;

namespace Moco.Skia.Backend;

/// <summary>
/// A skia backend for moco.
/// </summary>
public class SkiaMocoBackend : IMocoRendererBackend
{
    /// <inheritdoc/>
    public Action RenderFrameCallback { get; set; } = null!;

    /// <summary>
    /// The window.
    /// </summary>
    private readonly IWindow _window;

    /// <summary>
    /// The Skia 3d backing context.
    /// </summary>
    private readonly GRContext _context;

    /// <summary>
    /// The window render target.
    /// </summary>
    private GRBackendRenderTarget _rt = null!;

    /// <summary>
    /// The skia surface.
    /// </summary>
    private SKSurface _surface = null!;

    /// <summary>
    /// The background skia color.
    /// </summary>
    private SKColor _bgColor = SKColors.Black;

    /// <summary>
    /// The canvas.
    /// </summary>
    private SKCanvas _canvas;

    public SkiaMocoBackend()
    {
        // Create the Silk window that will be drawn into.
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Moco";
        options.PreferredStencilBufferBits = 8;
        options.PreferredBitDepth = new Vector4D<int>(8, 8, 8, 8);
        GlfwWindowing.Use();

        _window = Window.Create(options);
        _window.Initialize();

        // Create the Skia-GL layer.
        var grGlInterface = GRGlInterface.Create((name => _window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : 0));
        grGlInterface.Validate();

        _context = GRContext.CreateGl(grGlInterface);

        // Create the window render target.
        _rt = new GRBackendRenderTarget(800, 600, 0, 8, new GRGlFramebufferInfo(0, 0x8058));
        _surface = SKSurface.Create(_context, _rt, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
        _canvas = _surface.Canvas;

        _window.Render += d =>
        {
            _context.ResetContext();
            _canvas.Clear(_bgColor);

            RenderFrameCallback?.Invoke();

            _canvas.Flush();
        };
    }

    /// <summary>
    /// Runs the window.
    /// </summary>
    public void Run()
    {
        _window.Run();
    }

    /// <inheritdoc/>
    public void SetBackgroundClearColor(Rgba color)
    {
        _bgColor = new SKColor(color.Red, color.Green, color.Blue);
        Console.WriteLine($"Setting background color: {color.Red}, {color.Green}, {color.Blue}");
    }

    /// <inheritdoc/>
    public void SetWindowSize(SWF.DataTypes.Rectangle rect)
    {
        _window.Size = new Vector2D<int>(
            (int)rect.XMax.LogicalPixelValue, 
            (int)rect.YMax.LogicalPixelValue);

        _canvas.Dispose();
        _surface.Dispose();
        _rt.Dispose();

        _rt = new GRBackendRenderTarget(
            (int)rect.XMax.LogicalPixelValue, 
            (int)rect.YMax.LogicalPixelValue, 
            0, 8, new GRGlFramebufferInfo(1, 0x8058));

        _surface = SKSurface.Create(_context, _rt, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
        _canvas = _surface.Canvas;

        Console.WriteLine($"Setting window size: {rect.XMax.LogicalPixelValue}x{rect.YMax.LogicalPixelValue}");
    }

    /// <inheritdoc/>
    public object RegisterImageBytes(int id, int width, int height, byte[] bytes)
    {
        var bitmap = new SKBitmap();
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        bitmap.InstallPixels(
            info, 
            handle.AddrOfPinnedObject(), 
            info.RowBytes, 
            delegate { handle.Free(); }, 
            null);

        return bitmap;
    }

    /// <inheritdoc/>
    public IShape RegisterShape(int id, SWF.DataTypes.Rectangle bounds)
    {
        var bitmap = new SKBitmap(
            (int)(bounds.XMax.LogicalPixelValue - bounds.XMin.LogicalPixelValue),
            (int)(bounds.YMax.LogicalPixelValue - bounds.YMin.LogicalPixelValue));

        return new SkiaMocoShape(bitmap, null!, bounds, id);
    }

    /// <inheritdoc/>
    public void PlaceShape(IShape shape, Matrix matrix)
    {
        var skiaShape = shape as SkiaMocoShape;
        if (matrix.HasScale || matrix.HasRotation)
            throw new NotSupportedException("Support scaling or rotating drawn shapes.");

        _canvas.DrawBitmap(
            skiaShape!.Surface,
            new SKPoint(
                matrix.TranslateX.LogicalPixelValue + skiaShape.Bounds.XMin.LogicalPixelValue, 
                matrix.TranslateY.LogicalPixelValue + skiaShape.Bounds.YMin.LogicalPixelValue));
    }
}
