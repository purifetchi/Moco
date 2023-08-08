using Moco.SWF.DataTypes;

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
    void SetBackgroundClearColor(Rgb color);
}
