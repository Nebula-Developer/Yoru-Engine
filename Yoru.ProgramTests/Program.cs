#nullable disable

using System.Drawing;
using System.Numerics;
using System.Runtime.ExceptionServices;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class DraggableElement : Element {
    private MouseButton? curButton;
    
    private Vector2 mouseStart;
    private Vector2 startPos;
    public MouseButton Button { get; set; } = MouseButton.Left;
    
    protected override void OnLoad() {
        base.OnLoad();
    }
    
    protected override bool OnMouseDown(MouseButton button) {
        if (button != Button) return true;
        curButton = Button;
        base.OnMouseDown(button);
        mouseStart = App.Input.MousePosition;
        startPos = Transform.WorldPosition;
        return false;
    }
    
    protected override bool OnMouseUp(MouseButton button) {
        if (curButton == null || button != curButton) return true;
        curButton = null;
        base.OnMouseUp(button);
        return false;
    }
    
    protected override void OnMouseMove(Vector2 position) {
        if (curButton == null) return;
        base.OnMouseMove(position);
        Transform.WorldPosition = startPos + position - mouseStart;
    }
}

public class HoverText : TextElement {
    private bool locker;
    private Vector2 mouseStart;
    private SKColor selfColor;
    private Vector2 snapPos;
    
    protected override void OnLoad() {
        base.OnLoad();
        selfColor = Color;
    }
    
    protected override bool OnMouseDown(MouseButton button) {
        if (button != MouseButton.Left) return true;
        locker = true;
        base.OnMouseDown(button);
        Color = SKColors.Red;
        snapPos = Transform.WorldPosition;
        mouseStart = App.Input.MousePosition;
        return true;
    }
    
    protected override bool OnMouseUp(MouseButton button) {
        if (button != MouseButton.Left) return true;
        locker = false;
        base.OnMouseUp(button);
        Color = selfColor;
        return true;
    }
}

public class GridTestApp : Application {
    public Element fillGrid = new() {
        Transform = new() {
            Size = new(500),
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f)
        }
    };
    
    protected override void OnLoad() {
        base.OnLoad();
        
        for (var i = 0; i < 50; i++) {
            int inverseI = 50 - i;
            DraggableElement draggable = new() {
                Transform = new() {
                    Size = new(i * 10),
                    OffsetPosition = new(0.5f),
                    AnchorPosition = new(0.5f),
                    RotationOffset = new(0.5f)
                },
                ZIndex = inverseI,
                ClickThrough = true
            };
            
            var innerBoxColor = new SKColor((byte)(255 - (inverseI * 5)), (byte)(inverseI * 5), 0);
            var innerBoxColorHover = new SKColor((byte)(255 - (inverseI * 4.5f)), (byte)(inverseI * 4.5f), 255);

            var innerBox = new BoxElement {
                Color = innerBoxColor,
                Transform = new() {
                    ScaleWidth = true,
                    ScaleHeight = true,
                    AnchorPosition = new(0.5f),
                    OffsetPosition = new(0.5f)
                }
            };
            
            draggable.DoMouseEnter = () => innerBox.Color = innerBoxColorHover;
            draggable.DoMouseLeave = () => innerBox.Color = innerBoxColor;
            
            draggable.AddChild(innerBox);
            fillGrid.AddChild(draggable);
        }
        
        Element.AddChild(fillGrid);
    }

    protected override void OnUpdate() {
        base.OnUpdate();
        foreach (Element e in fillGrid.Children) {
            e.Transform.LocalRotation += 10f * (float)UpdateTime.DeltaTime;
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        GLWindow programTestWindow = new();
        Console.CancelKeyPress += (s, e) => {
            programTestWindow.Close();
            e.Cancel = true;
        };
        programTestWindow.App = new GridTestApp();
        programTestWindow.Run();
        Console.WriteLine("Program killed");
        programTestWindow.Dispose();
    }
}
