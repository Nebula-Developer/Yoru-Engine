#nullable disable

using System.Numerics;
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
        base.OnMouseDrag();
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
    
    protected override void OnMouseDrag() {
        if (!locker) return;
        base.OnMouseDrag();
        Transform.WorldPosition = snapPos + App.Input.MousePosition - mouseStart;
    }
}

public class GridTestApp : Application {
    public FillGridElement fillGrid = new() {
        RowSpacing = 3,
        ColumnSpacing = 3,
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
        
        for (var i = 0; i < 800; i++) {
            float size = 5;
            DraggableElement draggable = new() {
                Transform = new() {
                    Size = new(size)
                }
            };
            
            // draggable.AddChild(new BoxElement {
            //     Color = i % 2 == 0 ? SKColors.Blue : SKColors.Orange,
            //     Transform = new() {
            //         Size = new(size)
            //     }
            // });
            
            var innerBoxColor = i % 2 == 0 ? SKColors.Orange : SKColors.Blue;
            var innerBox = new BoxElement {
                Color = innerBoxColor,
                Transform = new() {
                    Size = new(size),
                    AnchorPosition = new(0.5f),
                    OffsetPosition = new(0.5f)
                }
            };
            
            innerBox.DoMouseEnter = () => {
                innerBox.Color = SKColors.Red;
                innerBox.Transform.Size = new(size * 4f);
                draggable.ZIndex = 999;
            };
            innerBox.DoMouseLeave = () => {
                innerBox.Color = innerBoxColor;
                innerBox.Transform.Size = new(size);
                draggable.ZIndex = 0;
            };
            
            draggable.AddChild(innerBox);
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
