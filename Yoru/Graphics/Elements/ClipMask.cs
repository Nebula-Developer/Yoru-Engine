using SkiaSharp;

namespace Yoru.Graphics.Elements;

public class ClipMask : Element {
    protected override void Render(SKCanvas canvas) =>
        canvas.ClipRect(new SKRect(0, 0, Transform.Size.X, Transform.Size.Y));
}