using System.Diagnostics;
using System.Numerics;
using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Input;
using Yoru.Platforms.GL;
using Yoru.Utilities;

public class TestApp : Application {
    public BoxElement Parent = new() {
        Transform = new() {
            // LocalPosition = new(300),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            Size = new(200),
            LocalRotation = 10,
            RotationOffset = new(0.5f)
        },
        Color = SKColors.Red,
    };

    public BoxElement Child = new() {
        Transform = new() {
            LocalPosition = new(0),
            AnchorPosition = new(1),
            OffsetPosition = new(1),
            Size = new(100),
            LocalRotation = 10,
            RotationOffset = new(1)
        },
        Color = SKColors.Blue
    };

    public BoxElement Child2 = new() {
        Transform = new() {
            LocalPosition = new(0),
            AnchorPosition = new(0),
            OffsetPosition = new(0),
            Size = new(50),
            LocalRotation = 10,
            RotationOffset = new(1)
        },
        Color = SKColors.Green
    };

    public BoxElement Child3 = new() {
        Transform = new() {
            LocalPosition = new(0),
            AnchorPosition = new(0),
            OffsetPosition = new(0),
            Size = new(25),
            LocalRotation = 10,
            RotationOffset = new(1)
        },
        Color = SKColors.Yellow
    };

    protected override void OnLoad() {
        base.OnLoad();

        Parent.AddChild(Child);
        Child.AddChild(Child2);
        Child2.AddChild(Child3);
        Element.AddChild(Parent);
    }

    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.X) {
            DrawingMethod = Enumerated.Next(DrawingMethod);
        }
    }

    protected override void OnRender() {
        base.OnRender();
        AppCanvas.DrawText("FPS: " + (1.0 / RenderTime.DeltaTime).ToString("0.00"), new SKPoint(10, 30), new SKPaint { Color = SKColors.White, TextSize = 20 });
    }

    protected override void OnUpdate() {
        base.OnUpdate();
        // rotate all children
        Parent.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 10;
        Child.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 10;
        Child2.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 10;
        Child3.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 10;
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GlWindow() {
            App = new TestApp()
        }.Run();
    }
}
