#nullable disable

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Platforms.GL;

public class GlRenderer : Renderer {
    private GRContext _context;
    private GRGlInterface _glInterface;
    private GRBackendRenderTarget _renderTarget;
    public IGLFWGraphicsContext GlContext { get; set; }
    
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
        _renderTarget = new(width, height, 0, 8, new(0, (uint)SizedInternalFormat.Rgba16f));
        _surface = SKSurface.Create(_context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.RgbaF16);
    }
    
    public override void Flush() {
        base.Flush();
        GlContext.SwapBuffers();
    }
}
