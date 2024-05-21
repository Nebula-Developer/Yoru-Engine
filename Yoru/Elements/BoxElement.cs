using SkiaSharp;

namespace Yoru.Elements;

public class BoxElement : PaintedElement {
    protected override void OnRender(SKCanvas canvas)
        => canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
}
