#nullable disable

using SkiaSharp;

namespace Yoru.Graphics;

public class Renderer : IDisposable {
    public SKCanvas Canvas => _surface.Canvas;
    public SKSurface Surface => _surface;

    protected SKSurface _surface;

    public virtual void Load() { }
    public virtual void Dispose() {
        _surface?.Dispose();
        GC.SuppressFinalize(this);
    }

    public virtual void Resize(int width, int height) { }
    public virtual void Flush() => _surface.Flush();
}
