#nullable disable

using System.Numerics;
using System.Reflection;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class MyApp : Application {
    public BoxElement bottomBar = new() {
        Color = SKColors.Orange,
        Transform = new() {
            Size = new(100),                 // Set the size to 100x100
            ScaleWidth = true,               // Always be the same width as the parent
            AnchorPosition = new(0.5f, 1),   // Anchor to the bottom center
            OffsetPosition = new(0.5f, 1),   // Offset to the bottom center
            RotationOffset = new(0.5f)       // Rotate around the center
        }
    };

    public BoxElement cursor = new() {
        Color = SKColors.White,
        Transform = new() {
            Size = new(5, 10),
            OffsetPosition = new(0.5f, 0.5f)
        }
    };

    protected override void OnMouseMove(Vector2 position) {
        base.OnMouseMove(position);
        cursor.Transform.LocalPosition = position;
    }

    protected override void OnLoad() {
        base.OnLoad();
        Element.AddChild(bottomBar);
        Element.AddChild(cursor);
    }

    bool toggle = false;
    protected override void OnMouseDown(MouseButton button) {
        base.OnMouseDown(button);

        toggle = !toggle;
        Vector2 startOffset = bottomBar.Transform.OffsetPosition;
        Vector2 endOffset = new(0.5f, toggle ? 0.5f : 1);
        Vector2 startSize = bottomBar.Transform.Size;
        float parentWidth = bottomBar.Parent.Transform.Size.X;
        Vector2 endSize = new(toggle ? 200 : parentWidth, toggle ? 200 : 100);
        bottomBar.Transform.ScaleWidth = !toggle;

        Animations.Add(new Animation() {
            Duration = 0.5f,
            Easing = Easing.ExpOut,
            OnUpdate = (t) => {
                bottomBar.Transform.OffsetPosition = Vector2.Lerp(startOffset, endOffset, (float)t);
                bottomBar.Transform.AnchorPosition = Vector2.Lerp(startOffset, endOffset, (float)t);
                bottomBar.Transform.Size = Vector2.Lerp(startSize, endSize, (float)t);
            }
        }, "jump");
    }

    protected override void OnRender() {
        AppCanvas.DrawRect(30, 30, Size.X - 60, Size.Y - 60, new SKPaint() {
            Color = new SKColor(100, 150, 200, 100)
        });
    }
}

public static class Program {
    public static void Main(string[] args) {
        GLWindow myWindow = new();
        myWindow.app = new MyApp();
        myWindow.Run();
        myWindow.Dispose();
    }
}
