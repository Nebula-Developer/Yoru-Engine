using SkiaSharp;

namespace Yoru.Elements;

public class RoundBoxElement : ColorableElement {
    public float BorderRadius = 0f;

    protected override void OnRender(SKCanvas canvas)
        => canvas.DrawRoundRect(0, 0, Transform.Size.X, Transform.Size.Y, BorderRadius, BorderRadius, Paint);
}
