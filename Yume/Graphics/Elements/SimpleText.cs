using SkiaSharp;

namespace Yume.Graphics.Elements;

public class SimpleText : ColorableElement {
    public string Text = "";

    public float FontSize {
        get => Paint.TextSize;
        set => Paint.TextSize = value;
    }

    protected override void Render(SKCanvas canvas) {
        canvas.DrawText(Text, 0, FontSize, Paint);
    }
}