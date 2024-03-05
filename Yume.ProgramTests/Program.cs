#pragma warning disable CS0162

using SkiaSharp;
using Yume.Graphics.Elements;
using Yume.Graphics.Windowing;

using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
 
namespace Yume;

public class InheritElement : Element {
    public SKColor Color {
        get => Paint.Color;
        set => Paint.Color = value;
    }

    public SKPaint Paint = new() {
        Color = SKColors.White
    };
    
    public override void Load() {
        base.Load();
        Console.WriteLine("Elm: " + this.GetHashCode());
    }

    public override void Render(SKCanvas canvas) {
        base.Render(canvas);
        canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
    }

    public override void Update() {
        base.Update();
        Transform.Size = new(Transform.Size.X, (float)(Window.UpdateTime.Time % 3f / 3f * 50) + 50f);
    }
}

public class InheritWindow : Window {
    public override void Load() {
        base.Load();
        Element.AddChild(new InheritElement() {
            Transform = {
                Size = new(50)
            }
        });
        
        Console.WriteLine("Win: " + this.GetHashCode());
    }

    public override void Update() {
        if (Element.Children[0] is InheritElement elm) {
            elm.Color = SKColors.Orange;
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        InheritWindow window = new() {
            UpdateFrequency = 144,
            RenderFrequency = 144
        };
        
        window.Run();
    }
}
