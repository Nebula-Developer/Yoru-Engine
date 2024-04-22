
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public class BoxElement : ColorableElement {
    protected override void Render(SKCanvas canvas)
        => canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
}