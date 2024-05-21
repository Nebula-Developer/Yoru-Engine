#nullable disable

using SkiaSharp;
using Yoru.Elements;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;

namespace Yoru.Example;

public class MyApp : Application {
    public readonly BoxElement Box = new() {
        Color = SKColors.Orange,
        Transform = new() {
            Size = new(100),            // Set the size to 100x100
            AnchorPosition = new(0.5f), // Anchor to its parent's center
            OffsetPosition = new(0.5f), // Offset by its own center
            RotationOffset = new(0.5f)  // Rotate around the center
        }
    };
    
    public readonly TextElement Text = new() {
        Text = "Try pressing the mouse or a key!",
        Color = SKColors.White,
        AutoResize = false, // Automaticaly resize the Transform's size to the text size
        TextSize = 20,
        Alignment = TextAlignment.Center,
        Transform = new() {
            Size = new(30),
            ScaleWidth = true // Always be the same width as the parent
        }
    };
    
    private bool toggleMouse, toggleKey;
    
    protected override void OnLoad() {
        base.OnLoad();
        Element.AddChild(Box);
        Element.AddChild(Text);
    }
    
    private float Lerp(float a, float b, float t) => a + (b - a) * t;
    
    protected override void OnMouseDown(MouseButton button) {
        base.OnMouseDown(button);
        
        toggleMouse = !toggleMouse;
        float rotation = toggleMouse ? 45 : 0;
        var currentRotation = Box.Transform.LocalRotation;
        
        Animations.Add(new() {
            Duration = 0.5f,
            Easing = Easing.ExpOut,
            OnUpdate = t => { Box.Transform.LocalRotation = Lerp(currentRotation, rotation, (float)t); }
        }, "rotate");
    }
    
    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        
        toggleKey = !toggleKey;
        float size = toggleKey ? 200 : 100;
        var currentSize = Box.Transform.Size.X;
        
        Animations.Add(new() {
            Duration = 0.5f,
            Easing = Easing.ExpOut,
            OnUpdate = t => { Box.Transform.Size = new(Lerp(currentSize, size, (float)t)); }
        }, "size");
    }
    
    // Manually drawing to the AppCanvas
    protected override void OnRender() {
        AppCanvas.DrawRect(30, 30, Size.X - 60, Size.Y - 60, new() {
            Color = new(100, 150, 200, 100)
        });
    }
}

public static class Program {
    public static void Main(string[] args) {
        GlWindow myWindow = new();
        myWindow.App = new MyApp();
        myWindow.Run();
        myWindow.Dispose();
    }
}
