
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public class CircleElement : ColorableElement {
    public float Radius {
        get => Transform.Size.X;
        set => Transform.Size = new(value);
    }

    protected override void Render(SKCanvas canvas)
        => canvas.DrawCircle(Radius / 2, Radius / 2, Radius / 2, Paint);
}
