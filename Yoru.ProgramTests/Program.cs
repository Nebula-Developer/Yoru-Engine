#nullable disable

using System.Globalization;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class Box : Element {
    public SKColor Color {
        get => Paint.Color;
        set => Paint.Color = value;
    }
    public SKPaint Paint = new() { Color = SKColors.White };

    protected override void Load() {
        Console.WriteLine("Loaded element: " + GetHashCode());
    }

    protected override void Render(SKCanvas canvas) {
        base.Render(canvas);
        canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
    }
}

public class ProgramTestApp : Application {
    public Box box = new() {
        Color = SKColors.Green,
        Transform = new() {
            Size = new(300),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            RotationOffset = new(0.5f)
        }
    };

    public Box childBox = new() {
        Color = SKColors.Red,
        Transform = new() {
            Size = new(100),
            AnchorPosition = new(1f),
            OffsetPosition = new(1f),
            RotationOffset = new(1f)
        }
    };

    public override void OnLoad() {
        base.OnLoad();
        Element.AddChild(box);
        box.AddChild(childBox);
    }

    public override void OnUpdate() {
        base.OnUpdate();
        box.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 90;
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
