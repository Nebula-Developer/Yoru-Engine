#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class DragPoint : DraggableElement {
    protected override void OnRender(SKCanvas canvas) {
        base.OnRender(canvas);
        canvas.DrawCircle(Transform.Size.X / 2, Transform.Size.X / 2, Transform.Size.X / 2, new() {
            Color = SKColors.Red,
            IsAntialias = true
        });
    }
}

public class GridTestApp : Application {
    public CircleElement circTest = new() {
        Color = SKColors.Cyan,
        Transform = new() {
            WorldPosition = new(200, 200),
            Size = new(100),
            // AnchorPosition = new(1),
            OffsetPosition = new(1)
        },
        ZIndex = 999
    };
    
    public PathElement path = new() {
        Color = SKColors.White,
        StrokeWidth = 20,
        FillType = SKPathFillType.EvenOdd,
        PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0),
        Points = [
            new(100, 100),
            new(300, 300),
            new(300, 100)
        ]
    };
    public DragPoint pointA = new() {
        Transform = new() {
            WorldPosition = new(100, 100),
            Size = new(8),
            OffsetPosition = new(0.5f)
        }
    };
    
    public DragPoint pointB = new() {
        Transform = new() {
            WorldPosition = new(300, 300),
            Size = new(8),
            OffsetPosition = new(0.5f)
        }
    };
    
    public DragPoint pointC = new() {
        Transform = new() {
            WorldPosition = new(300, 100),
            Size = new(8),
            OffsetPosition = new(0.5f)
        }
    };
    
    protected override void OnLoad() {
        Element.AddChild(pointA);
        Element.AddChild(pointB);
        Element.AddChild(pointC);
        Element.AddChild(path);
        Element.AddChild(circTest);
        
        void Update() {
            List<Vector2> points = new();
            var a = pointA.Transform.WorldPosition;
            var b = pointB.Transform.WorldPosition;
            var c = pointC.Transform.WorldPosition;
            points.Add(a);
            
            for (float t = 0; t <= 1; t += 0.01f) {
                var ab = Lerp(a, b, t);
                var bc = Lerp(b, c, t);
                var abc = Lerp(ab, bc, t);
                
                points.Add(abc);
            }
            
            points.Add(c);
            path.Points = points.ToArray();
        }
        
        pointA.DoMouseMove += _ => Update();
        pointB.DoMouseMove += _ => Update();
        pointC.DoMouseMove += _ => Update();
    }
    
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => new(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
    
    protected override void OnUpdate() {
        if (Input.GetKeyDown(Key.Space)) {
            DrawingMethod = Enumerated.Next(DrawingMethod);
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
