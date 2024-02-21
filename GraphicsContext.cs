using SkiaSharp;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class GraphicsContext(Window window) : WindowContext(window) {
    #nullable disable
    private GRGlInterface _interface;
    private GRContext _context;

    private GRBackendRenderTarget _renderTarget;
    public SKSurface Surface;
    #nullable restore

    public void Load() {
        _interface = GRGlInterface.Create();
        _context = GRContext.CreateGl(_interface);
        Resize(Window.FramebufferSize);
    }

    public void Resize(Vector2i size) => Resize(size.X, size.Y);
    public void Resize(int width, int height) {
        _renderTarget?.Dispose();
        _renderTarget = new GRBackendRenderTarget(width, height, 0, 8, new(0, (uint)SizedInternalFormat.Rgba8));
        
        Surface?.Dispose();
        Surface = SKSurface.Create(_context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
    }

    public void Dispose() {
        _renderTarget?.Dispose();
        Surface?.Dispose();
        _context?.Dispose();
        _interface?.Dispose();
    }
}
