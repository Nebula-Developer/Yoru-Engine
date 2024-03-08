using OpenTK.Mathematics;
using SkiaSharp;

namespace Yume.Graphics.Elements;

public class SimpleText : ColorableElement {
    public bool AutoScale = true;
    
    public string Text {
        get => _text;
        set {
            _text = value;
            if (AutoScale)
                Transform.Size = new Vector2(Paint.MeasureText(value), FontSize);
        }
    }
    
    private string _text = "";

    public float FontSize {
        get => Paint.TextSize;
        set => Paint.TextSize = value;
    }

    protected override void Render(SKCanvas canvas) =>
        canvas.DrawText(Text, 0, FontSize, Paint);
}