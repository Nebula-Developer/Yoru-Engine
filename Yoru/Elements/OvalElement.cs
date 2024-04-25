using SkiaSharp;

namespace Yoru.Elements;

public class OvalElement : ColorableElement {
    protected override void OnRender(SKCanvas canvas)
        => canvas.DrawOval(Transform.Size.X / 2, Transform.Size.Y / 2, Transform.Size.X / 2, Transform.Size.Y / 2, Paint);
}
