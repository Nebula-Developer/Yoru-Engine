using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;
using Yoru.Windowing;

namespace Yoru.Graphics;

public class GraphicsContext(Window? window) : WindowContext(window) {
#nullable disable
    private GRGlInterface _interface;
    private GRContext _context;
    private GRBackendRenderTarget _renderTarget;

    /// <summary>
    ///     The SkiaSharp surface used for drawing
    /// </summary>
    public SKSurface Surface;
#nullable restore

    public void Load() {
        if (Window == null)
            throw new InvalidOperationException("Cannot load graphics context without a window");

        _interface = GRGlInterface.Create();
        _context = GRContext.CreateGl(_interface);
        Resize(Window.FramebufferSize);
    }

    public void Resize(Vector2i size) {
        Resize(size.X, size.Y);
    }

    public void Resize(int width, int height) {
        _renderTarget?.Dispose();
        _renderTarget = new GRBackendRenderTarget(width, height, 0, 8,
            new GRGlFramebufferInfo(0, (uint)SizedInternalFormat.Rgba8));

        Surface?.Dispose();
        Surface = SKSurface.Create(_context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888,
            new SKSurfaceProperties(SKPixelGeometry.Unknown));
    }

    public void Dispose() {
        _renderTarget?.Dispose();
        Surface?.Dispose();
        _context?.Dispose();
        _interface?.Dispose();
    }
}