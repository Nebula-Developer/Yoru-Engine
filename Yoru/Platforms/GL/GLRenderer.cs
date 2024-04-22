#nullable disable

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Platforms.GL;

public class GLRenderer : Renderer {
    internal GRGlInterface _glInterface;
    internal GRContext _context;
    internal GRBackendRenderTarget _renderTarget;
    public IGLFWGraphicsContext GLContext { get; set; }

    public override void Load() {
        base.Load();
        _glInterface = GRGlInterface.Create();
        _context = GRContext.CreateGl(_glInterface);
    }

    public override void Dispose() {
        base.Dispose();
        _context?.Dispose();
        _glInterface?.Dispose();
        _renderTarget?.Dispose();
    }

    public override void Resize(int width, int height) {
        base.Resize(width, height);
        _surface?.Dispose();
        _renderTarget?.Dispose();
        _renderTarget = new GRBackendRenderTarget(width, height, 0, 8, new GRGlFramebufferInfo(0, (uint)SizedInternalFormat.Rgba8));
        _surface = SKSurface.Create(_context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
    }

    public override void Flush() {
        base.Flush();
        GLContext.SwapBuffers();
    }
}
