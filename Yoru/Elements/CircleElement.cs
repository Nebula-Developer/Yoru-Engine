using SkiaSharp;

namespace Yoru.Elements;

public class CircleElement : ColorableElement {
    public float Radius {
        get => Transform.Size.X;
        set => Transform.Size = new(value);
    }
    
    protected override void OnRender(SKCanvas canvas)
        => canvas.DrawCircle(Radius / 2, Radius / 2, Radius / 2, Paint);
}
