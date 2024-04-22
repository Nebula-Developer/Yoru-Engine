
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public enum TextAlignment {
    Left,
    Center,
    Right
}

public class TextElement : ColorableElement {
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

    public bool AutoResize = true;

    public string Text {
        get => _text;
        set {
            _text = value;
            if (AutoResize)
                Transform.Size = new(Paint.MeasureText(_text), Paint.TextSize);
        }
    }
    private string _text = "";

    public TextAlignment Alignment = TextAlignment.Left;

    protected override void Render(SKCanvas canvas) {
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
