
using SkiaSharp;

namespace Yoru;

public class CanvasContext(Application app) : AppContext(app) {
    public SKSurface? Surface;

    public bool IgnoreOwnSurface = false;
    public bool UseRendererCanvas => IgnoreOwnSurface || Surface == null;
    public SKCanvas Canvas => UseRendererCanvas ? App.Renderer.Canvas : Surface!.Canvas;
}
