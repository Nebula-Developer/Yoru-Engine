// Used to test dynamic loading of elements from separate assemblies


using System;
using SkiaSharp;
using Yoru.Graphics;
using Yoru.Graphics.Elements;
using Yoru.Windowing;


namespace Internal;

public class Inner : Element {
    protected override void Load() {
        base.Load();
        Cull = false;
    }


    protected override void Render(SKCanvas canvas) {
        double percent = Math.Abs(Math.Sin(Window.RenderTime.Time * 1));
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(10, 100, 200, 200), 10, 10), new SKPaint() {
            Color = new SKColor((byte)(percent * 255), 100, 100)
        });
    }
}

public static class Static {
    private static double _t;

    public static Window? Window;
    public static DateTime Begin;


    private static SKPaint _textPaint = new SKPaint {
        Color = SKColors.White,
        TextSize = 20
    };

    public static void Render(SKCanvas canvas) {
        canvas.DrawText("INTERNAL T = " + Math.Round(_t, 1), 10, 30, _textPaint);
        canvas.DrawText("CONSTANT T = " + Math.Round((DateTime.Now - Begin).TotalSeconds, 1), 10, 60, _textPaint);


        if (_t == 0) Begin = DateTime.Now;
        _t += Window!.RenderTime.DeltaTime;
    }
}