using Moco.Rasterization;
using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.Rendering;

/// <summary>
/// A rendering backend for Moco.
/// </summary>
public interface IMocoRendererBackend
{
    /// <summary>
    /// Sets the window size.
    /// </summary>
    /// <param name="rect">The window size.</param>
    void SetWindowSize(Rectangle rect);

    /// <summary>
    /// Sets the background clear color.
    /// </summary>
    /// <param name="color">The clear color.</param>
    void SetBackgroundClearColor(Rgba color);

    /// <summary>
    /// Registers image bytes.
    /// </summary>
    /// <param name="id">The id to register them under.</param>
    /// <param name="data">The data.</param>
    void RegisterImageBytes(int id, byte[] data);

    /// <summary>
    /// Registers a shape.
    /// </summary>
    /// <param name="id">The shape id.</param>
    /// <returns>A drawing context to rasterize said shape.</returns>
    IMocoDrawingContext RegisterShape(int id, Rectangle bounds);
}
