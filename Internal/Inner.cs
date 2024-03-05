// Used to test dynamic loading of elements from separate assemblies

using SkiaSharp;

namespace Internal;

public class Inner : Element {
    public override void Load() {
        base.Load();
    }

    public override void Render(SKCanvas canvas) {
        base.Render(canvas);
        float percent = ((Window.RenderTime.Time % 10) / 10);
        canvas.Clear(new SKColor((byte)(percent * 255), 100, 100));
    }
}

public static class Static {
    static int t = 0;
    public static void Render(SKCanvas canvas) {
        canvas.Clear(SKColors.Orange);
        canvas.DrawText("T=" + t, 10, 10, new SKPaint() {
            Color = SKColors.Black,
            TextSize = 20
        });

        t++;
    }
}