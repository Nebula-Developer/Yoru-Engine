#nullable disable

using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class ProgramTestApp : Application {
    public BoxElement box = new() {
        Color = SKColors.Green,
        Transform = new() {
            Size = new(300),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            RotationOffset = new(0.5f)
        }
    };

    public BoxElement childBox = new() {
        Color = SKColors.Red,
        Transform = new() {
            Size = new(100),
            AnchorPosition = new(1f),
            OffsetPosition = new(1f),
            RotationOffset = new(1f)
        }
    };

    public List<CircleElement> circles = new();

    public CircleElement background = new() {
        Color = SKColors.Aqua,
        ZIndex = -1000,
        Transform = new() {
            ScaleHeight = true,
            ScaleWidth = true
        }
    };

    public TextElement text = new() {
        Text = "Hello, World!",
        Color = SKColors.White,
        Transform = new() {
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f)
        }
    };

    public override void OnLoad() {
        base.OnLoad();
        Element.AddChild(box);
        Element.AddChild(background);
        box.AddChild(childBox);
        text.Parent = childBox;
        text.TextSize = 30;

        for (int i = 0; i < 10; i++) {
            CircleElement circle = new();

            circle.Color = SKColors.Magenta;
            circle.Transform.Size = new(100);

            Element.AddChild(circle);
            circles.Add(circle);
        }

        Animations.Add(new Animation() {
            Duration = 5,
            LoopMode = AnimationLoopMode.Mirror,
            Easing = (e) => Math.Pow(e, 3),
            OnUpdate = (double p) => {
                box.Transform.LocalRotation = (float)p * 360;
            }
        });
    }

    public override void OnMouseMove(System.Numerics.Vector2 position) {
        base.OnMouseMove(position);
        for (int i = 0; i < circles.Count; i++)
            circles[i].Transform.LocalPosition = position * ((i + 4f) / 2f);
    }

    public override void OnUpdate() {
        base.OnUpdate();
        childBox.Transform.LocalRotation -= (float)UpdateTime.DeltaTime * 180;

        if (Input.GetMouseButtonDown(MouseButton.Left))
            box.Color = SKColors.Orange;
        
        if (Input.GetMouseButtonUp(MouseButton.Left))
            box.Color = SKColors.Green;
    }

    bool toggle = false;
    public override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.Escape) Close();

        if (key == Key.Space) {
            toggle = !toggle;
            childBox.Transform.RotationOffset = toggle ? new(0.5f) : new(1f);
            childBox.Transform.OffsetPosition = toggle ? new(0.5f) : new(1f);
            childBox.Transform.AnchorPosition = toggle ? new(0.5f) : new(1f);
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        GLWindow programTestWindow = new();
        Console.CancelKeyPress += (s, e) => { programTestWindow.Close(); e.Cancel = true; };
        programTestWindow.app = new ProgramTestApp();
        programTestWindow.Run();
        Console.WriteLine("Program killed");
        programTestWindow.Dispose();
    }
}
