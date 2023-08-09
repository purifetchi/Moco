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

    /// <summary>
    /// Registers image bytes.
    /// </summary>
    /// <param name="id">The id to register them under.</param>
    /// <param name="data">The data.</param>
    void RegisterImageBytes(int id, byte[] data);
}
