using SkiaSharp;

namespace Yume.Graphics.Elements;

public class ColorableElement : Element {
    public SKPaint Paint = new() {
        Color = SKColors.White
    };

    public SKColor Color {
        get => Paint.Color;
        set => Paint.Color = value;
    }
}