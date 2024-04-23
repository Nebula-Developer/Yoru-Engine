using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public abstract class ColorableElement : Element {
    public SKPaint Paint = new() {
        Color = SKColors.White,
        IsAntialias = true,
        FilterQuality = SKFilterQuality.Low
    };
    
    public SKColor Color {
        get => Paint.Color;
        set => Paint.Color = value;
    }
}
