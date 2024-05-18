#nullable disable

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using Silk.NET.OpenGL;
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Platforms.SDL;

public class SDLRenderer : Renderer {
    internal GRContext _context;

    public override void Load() {
        base.Load();
        _context = GRContext.CreateGl();
        Resize(800, 600);
    }

    public override void Resize(int width, int height) {
        base.Resize(width, height);

        _surface = SKSurface.Create(_context,
                new GRBackendRenderTarget(width, height, 0, 8, new GRGlFramebufferInfo(0, (uint)GLEnum.Rgba8)),
                SKColorType.Rgba8888);
    }
}
