using Moco.Rendering;
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

        _window.Render += d =>
        {
            using var canvas = _surface.Canvas;

            _context.ResetContext();
            canvas.Clear(_bgColor);

            canvas.Flush();
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
    public void SetBackgroundClearColor(Rgb color)
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

        _surface.Dispose();
        _rt.Dispose();

        _rt = new GRBackendRenderTarget(
            (int)rect.XMax.LogicalPixelValue, 
            (int)rect.YMax.LogicalPixelValue, 
            0, 8, new GRGlFramebufferInfo(0, 0x8058));

        _surface = SKSurface.Create(_context, _rt, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);

        Console.WriteLine($"Setting window size: {rect.XMax.LogicalPixelValue}x{rect.YMax.LogicalPixelValue}");
    }

    /// <inheritdoc/>
    public void RegisterImageBytes(int id, byte[] bytes)
    {

    }
}
