using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Platforms.GL;

public class MyApp : Application {
    private readonly BoxElement box = new() {
        Color = SKColors.Green,
        Transform = new() {
            Size = new(100),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            RotationOffset = new(0.5f)
        }
    };
    
    protected override void OnLoad() {
        base.OnLoad();
        Element.AddChild(box);
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        box.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 100f;
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GLWindow {
            App = new MyApp()
        }.Run();
    }
}
