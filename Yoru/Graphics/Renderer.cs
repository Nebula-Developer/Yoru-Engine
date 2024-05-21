#nullable disable

using SkiaSharp;

namespace Yoru.Graphics;

public abstract class Renderer : IDisposable {
    // ReSharper disable once InconsistentNaming
    protected SKSurface _surface;
    public SKCanvas Canvas {
        get => _surface.Canvas;
    }
    public SKSurface Surface {
        get => _surface;
    }
    public virtual void Dispose() {
        _surface?.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public virtual void Load() { }
    
    public virtual void Resize(int width, int height) { }
    public virtual void Flush() => _surface.Flush();
}
