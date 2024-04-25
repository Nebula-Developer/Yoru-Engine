#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class DraggableElement : Element {
    public MouseButton Button { get; set; } = MouseButton.Left;
    private MouseButton? curButton;

    private Vector2 mouseStart;
    private Vector2 startPos;

    protected override void Load() {
        base.Load();
        MouseInteractions = true;
    }

    public override bool MouseDown(MouseButton button) {
        if (button != Button) return true;
        curButton = Button;
        base.MouseDown(button);
        mouseStart = App.Input.MousePosition;
        startPos = Transform.WorldPosition;
        return false;
    }

    public override bool MouseUp(MouseButton button) {
        if (curButton == null || button != curButton) return true;
        curButton = null;
        base.MouseUp(button);
        return false;
    }

    public override void MouseDrag() {
        if (curButton == null) return;
        base.MouseDrag();
        Transform.WorldPosition = startPos + App.Input.MousePosition - mouseStart;
    }
}

public class HoverText : TextElement {
    private Vector2 mouseStart;
    private SKColor selfColor;
    private Vector2 snapPos;
    
    protected override void Load() {
        base.Load();
        selfColor = Color;
        MouseInteractions = true;
    }

    bool locker = false;
    
    public override bool MouseDown(MouseButton button) {
        if (button != MouseButton.Left) return true;
        locker = true;
        base.MouseDown(button);
        Color = SKColors.Red;
        snapPos = Transform.WorldPosition;
        mouseStart = App.Input.MousePosition;
        return true;
    }
    
    public override bool MouseUp(MouseButton button) {
        if (button != MouseButton.Left) return true;
        locker = false;
        base.MouseUp(button);
        Color = selfColor;
        return true;
    }
    
    public override void MouseDrag() {
        if (!locker) return;
        base.MouseDrag();
        Transform.WorldPosition = snapPos + App.Input.MousePosition - mouseStart;
    }
}

public class HoverBox : BoxElement {
    protected override void Load() {
        base.Load();
        MouseInteractions = true;
    }
    SKColor oldColor;
    public override void MouseEnter() {
        base.MouseEnter();
        oldColor = Color;
        Color = SKColors.Magenta;
    }

    public override void MouseLeave() {
        base.MouseLeave();
        Color = oldColor;
    }
}

public class GridTestApp : Application {
    public FillGridElement fillGrid = new() {
        RowSpacing = 10,
        ColumnSpacing = 10,
        Transform = new() {
            ScaleWidth = true,
            ScaleHeight = true
        }
    };
    public GridElement grid = new() {
        MaxRows = 4,
        MaxColumns = 4,
        RowSpacing = 10,
        ColumnSpacing = 10,
        ElementWidth = 100,
        ElementHeight = 100
    };
    public HoverText text = new() {
        Transform = new() {
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f)   
        },
        TextSize = 40,
        Color = SKColors.White,
        Text = "Hello"
    };
    
    protected override void OnLoad() {
        base.OnLoad();
        for (var i = 0; i < 16; i++) {
            grid.AddChild(new HoverBox {
                Color = i % 2 == 0 ? SKColors.Red : SKColors.Green,
                Transform = new() {
                    Size = new(100)
                }
            });
        }
        
        for (var i = 0; i < 16; i++) {
            float size = new Random().Next(50, 200);
            DraggableElement draggable = new() {
                Transform = new() {
                    Size = new(size)
                }
            };

            draggable.AddChild(new HoverBox {
                Color = i % 2 == 0 ? SKColors.Blue : SKColors.Orange,
                Transform = new() {
                    Size = new(size)
                }
            });

            draggable.AddChild(new HoverBox {
                Color = i % 2 == 0 ? SKColors.Orange : SKColors.Blue,
                Transform = new() {
                    Size = new(size - 5),
                    AnchorPosition = new(0.5f),
                    OffsetPosition = new(0.5f)
                }
            });

            fillGrid.AddChild(draggable);
        }
        
        Element.AddChild(fillGrid);
        Element.AddChild(text);
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        grid.RowSpacing = 10 + (float)Math.Sin(UpdateTime.Time) * 10;
        grid.ColumnSpacing = 10 + (float)Math.Cos(UpdateTime.Time) * 10;
    }
    
    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.Up) grid.MaxRows++;
        if (key == Key.Down) grid.MaxRows--;
        if (key == Key.Left) grid.MaxColumns--;
        if (key == Key.Right) grid.MaxColumns++;
        if (key == Key.X) {
            grid.FlowDirection = grid.FlowDirection == GridFlowDirection.Column ? GridFlowDirection.Row : GridFlowDirection.Column;
            fillGrid.FlowDirection = fillGrid.FlowDirection == GridFlowDirection.Column ? GridFlowDirection.Row : GridFlowDirection.Column;
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
