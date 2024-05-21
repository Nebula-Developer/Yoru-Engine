using SkiaSharp;

namespace Yoru.Elements;

public enum TextAlignment {
    Left,
    Center,
    Right
}

public class TextElement : PaintedElement {
    private string _text = "";
    
    public TextAlignment Alignment = TextAlignment.Left;
    
    public bool AutoResize = true;
    public float TextSize {
        get => Paint.TextSize;
        set {
            Paint.TextSize = value;
            if (AutoResize)
                Transform.Size = new(Paint.MeasureText(Text), Paint.TextSize);
        }
    }
    
    public SKTypeface Typeface {
        get => Paint.Typeface;
        set => Paint.Typeface = value;
    }
    
    public string Text {
        get => _text;
        set {
            _text = value;
            if (AutoResize)
                Transform.Size = new(Paint.MeasureText(_text), Paint.TextSize);
        }
    }
    
    protected override void OnRender(SKCanvas canvas) {
        float x = 0;
        switch (Alignment) {
            case TextAlignment.Center:
                x = Transform.Size.X / 2 - Paint.MeasureText(Text) / 2;
                break;
            case TextAlignment.Right:
                x = Transform.Size.X - Paint.MeasureText(Text);
                break;
        }
        
        canvas.DrawText(Text, x, Paint.TextSize, Paint);
    }
}
