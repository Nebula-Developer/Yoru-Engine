using SkiaSharp;

namespace Yume.Graphics.Elements;

public class Box : ColorableElement {
    protected override void Render(SKCanvas canvas) =>
        canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
}