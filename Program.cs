#pragma warning disable CS0162

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

public class MyWindow : Window {
    private SimpleText FPSText = new();


    BoxElement center = new();

    public override void Load() {
        base.Load();

        center.Color = SKColors.Orange;
        center.ZIndex = 999;
        center.Transform.OffsetPosition = new(0.5f, 0.5f);
        center.Transform.AnchorPosition = new(0.5f, 0.5f);
        center.Transform.Size = new(100, 100);
        Element.AddChild(center);

        BoxElement _box = new();
        _box.Transform.AnchorPosition = new(1, 1);
        _box.Transform.OffsetPosition = new(1, 1);
        _box.Transform.ScaleWidth = true;
        _box.Transform.Size = new(0, 50);
        _box.Color = SKColors.White;
        Element.AddChild(_box);

        BoxElement inner = new();
        inner.Transform.AnchorPosition = new(0.5f, 0.5f);
        inner.Transform.OffsetPosition = new(0.5f, 0.5f);
        inner.Transform.ScaleHeight = true;
        inner.Transform.Size = new(50, 0);
        inner.Color = SKColors.Red;
        _box.AddChild(inner);

        BoxElement background = new();
        background.Transform.ParentScale = Vector2.One;
        background.Color = SKColors.Gray;
        background.ZIndex = -1;
        Element.AddChild(background);

        BoxElement top = new();
        top.Transform.ScaleWidth = true;
        top.Transform.Size = new(0, 50);
        top.Color = SKColors.White;
        top.ZIndex = 1;
        Element.AddChild(top);

        FPSText.Transform.WorldPosition = new(100, 100);
        FPSText.TextColor = SKColors.White;
        FPSText.FontSize = 20;
        FPSText.ZIndex = 100;
        Element.AddChild(FPSText);
    }

    public override void Render() {
        base.Render();
        FPSText.Text = "FPS: " + (1 / RenderTime.DeltaTime).ToString("0.00");
    }

    public override void Update() {
        base.Update();

        float time = UpdateTime.Time;
        float valMod = 1 - (time % 0.5f / 0.5f);
        center.Transform.Size = new(50 + (valMod * 50));
    }

    public override void KeyDown(KeyboardKeyEventArgs e) {
        base.KeyDown(e);

        if (e.Key == Keys.Space) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine("Multithreaded: " + IsMultiThreaded);
        }

        else if (e.Key == Keys.Down) {
            UpdateFrequency -= 50;
            RenderFrequency -= 50;
            Console.WriteLine("Update/Render Frequency: " + UpdateFrequency);
        }

        else if (e.Key == Keys.Up) {
            UpdateFrequency += 50;
            RenderFrequency += 50;
            Console.WriteLine("Update/Render Frequency: " + UpdateFrequency);
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        MyWindow window = new();
        window.UpdateFrequency = 144;
        window.RenderFrequency = 144;

        window.Run();
    }
}
