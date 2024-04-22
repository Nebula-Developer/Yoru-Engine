
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public class OvalElement : ColorableElement {
    protected override void Render(SKCanvas canvas)
        => canvas.DrawOval(Transform.Size.X / 2, Transform.Size.Y / 2, Transform.Size.X / 2, Transform.Size.Y / 2, Paint);
}
