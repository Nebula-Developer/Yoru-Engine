using System;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

public class MyWindow : Window {
    Stopwatch resizeTimer = new();

    public override void Render() {
        base.Render();

        Canvas.DrawRect(0, 0, MathF.Abs(MathF.Sin(RenderTime.Time)) * FramebufferSize.X, 100, new SKPaint() {
            Color = SKColors.Blue
        });

        int fps = (int)(1 / RenderTime.RawDeltaTime);
        Canvas.DrawText($"FPS: {fps}", 10, 60, new SKPaint() {
            Color = SKColors.White,
            TextSize = 30
        });

        if (resizeTimer.IsRunning && resizeTimer.ElapsedMilliseconds < 2000) {
            // get % (1-0) of 1500 to 2000
            float progToFull = MathF.Max((resizeTimer.ElapsedMilliseconds - 1500) / 500f, 0);

            Canvas.DrawText("Resize: " + Size.X + "x" + Size.Y, 10, 30 - (70 * progToFull), new SKPaint() {
                Color = SKColors.White,
                TextSize = 40,
                Typeface = SKTypeface.FromFamilyName("Arial")
            });
        }

        RenderTime.TimeScale = (MathF.Sin(RenderTime.RawTime / 5f) * 5) + 0.1f;
    }

    public override void KeyDown(KeyboardKeyEventArgs e) {
        base.KeyDown(e);

        if (e.Key == Keys.Space) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine(IsMultiThreaded);
        }

        else if (e.Key == Keys.Down) {
            UpdateFrequency -= 10;
            RenderFrequency -= 10;
        }

        else if (e.Key == Keys.Up) {
            UpdateFrequency += 10;
            RenderFrequency += 10;
        }
    }

    public override void Resize(Vector2i size) {
        base.Resize(size);
        resizeTimer.Restart();
    }
}

public static class Program {
    public static void Main(string[] args) {
        MyWindow window = new();
        window.UpdateFrequency = 60;
        window.RenderFrequency = 60;
        window.Run();
    }
}
