using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public abstract class PaintedElement : Element {
    public SKPaint Paint = new() {
        Color = SKColors.White,
        IsAntialias = true,
        FilterQuality = SKFilterQuality.Low
    };
    
    public SKShader Shader {
        get => Paint.Shader;
        set => Paint.Shader = value;
    }
    
    public SKColor Color {
        get => Paint.Color;
        set => Paint.Color = value;
    }
}
