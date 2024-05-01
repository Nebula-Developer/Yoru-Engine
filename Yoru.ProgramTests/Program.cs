#nullable disable

using System.Drawing;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Transactions;
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
        return true;
    }
    
    protected override bool OnMouseUp(MouseButton button) {
        if (curButton == null || button != curButton) return true;
        curButton = null;
        base.OnMouseUp(button);
        return true;
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

        BoxElement topBar = new() {
            Transform = new() {
                Size = new(40),
                ScaleWidth = true
            },
            Color = new(120, 120, 120),
            ClickThrough = true
        };

        Element fillGridParent = new (){
            Transform = new() {
                LocalPosition = new(5, 0),
                AnchorPosition = new(0, 0.5f),
                OffsetPosition = new(0, 0.5f),
                ScaleHeight = true,
                ScaleWidth = true
            }
        };

        FillGridElement leftGrid = new() {
            Transform = new() {
                ScaleHeight = true,
                ScaleWidth = true
            },
            FlowDirection = GridFlowDirection.Row,
            ColumnSpacing = 10,
            AutoRemap = false
        };

        fillGridParent.AddChild(leftGrid);

        leftGrid.DoMouseDown += (btn) => {
            leftGrid.RemapGrid();
        };

        for (int i = 0; i < 5; i++) {
            var drag = new DraggableElement() {
                Transform = new() {
                    Size = new(30),
                    AnchorPosition = new(0, 0.5f),
                    OffsetPosition = new(0, 0.5f)
                },
                ClickThrough = false,
                Button = i % 2 == 0 ? MouseButton.Left : MouseButton.Right
                // Color = SKColors.Red
            };

            var box = new BoxElement() {
                Transform = new() {
                    ScaleWidth = true,
                    ScaleHeight = true,
                },
                Color = SKColors.Red
            };

            box.Parent = drag;

            bool locker = false;
            bool over = false;
            drag.DoMouseEnter += () => {
                over = true;
                box.Color = SKColors.Orange;
            };

            drag.DoMouseDown += (btn) => {
                if (btn != drag.Button) return;
                locker = true;
            };

            drag.DoMouseLeave += () => {
                over = false;
                if (locker) return;
                box.Color = SKColors.Red;
            };

            drag.DoMouseUp += (btn) => {
                if (!locker || btn != drag.Button || over) {
                    locker = false;
                    return;
                }
                box.Color = SKColors.Red;
                locker = false;
            };

            leftGrid.AddChild(drag);
        }

        topBar.AddChild(fillGridParent);

        topBar.DoMouseEnter += () => {
            topBar.Color = new(150, 150, 150);
        };

        topBar.DoMouseLeave += () => {
            topBar.Color = new(120, 120, 120);
        };

        Element.AddChild(topBar);
    }   

    protected override void OnUpdate() {
        base.OnUpdate();
        foreach (Element e in fillGrid.Children) {
            // e.Transform.LocalRotation += 10f * (float)UpdateTime.DeltaTime;
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
