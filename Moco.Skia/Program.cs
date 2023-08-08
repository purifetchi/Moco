using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using SkiaSharp;

var options = WindowOptions.Default;
options.Size = new Vector2D<int>(800, 600);
options.Title = "Test Skia window";
options.PreferredStencilBufferBits = 8;
options.PreferredBitDepth = new Vector4D<int>(8, 8, 8, 8);
GlfwWindowing.Use();

var window = Window.Create(options);
window.Initialize();

using var grGlInterface = GRGlInterface.Create((name => window.GLContext!.TryGetProcAddress(name, out var addr) ? addr : 0));
grGlInterface.Validate();

using var grContext = GRContext.CreateGl(grGlInterface);
var rt = new GRBackendRenderTarget(800, 600, 0, 8, new GRGlFramebufferInfo(0, 0x8058));

using var surface = SKSurface.Create(grContext, rt, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
using var canvas = surface.Canvas;

window.Render += d =>
{
    grContext.ResetContext();
    canvas.Clear(SKColors.Black);

    using var paint = new SKPaint();
    paint.Color = SKColors.White;
    canvas.DrawCircle(150, 150, 100, paint);
    canvas.Flush();
};

window.Run();