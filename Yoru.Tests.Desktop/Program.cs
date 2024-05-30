using System.Numerics;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Input;
using Yoru.Platforms.GL;
using Yoru.Utilities;

namespace Yoru.Tests.Desktop;

public class SnowflakeOverlay : PaintedElement {
    public int Count = 100;
    public List<Tuple<Vector2, float>> Points = new(); // they are from 0 to 1
    public float Scale = 1f;
    
    protected override void OnRender(SKCanvas canvas) {
        for (var i = 0; i < Points.Count; i++) {
            var pos = Points[i].Item1 * new Vector2(Transform.Size.X, Transform.Size.Y + Scale * 2) - new Vector2(0, Scale);
            canvas.DrawCircle(new(pos.X, pos.Y), Scale, Paint);
            
            Points[i] = new(Points[i].Item1 + new Vector2(0, Points[i].Item2) * (float)App.UpdateTime.DeltaTime, Points[i].Item2);
            if (Points[i].Item1.Y > 1) Points.RemoveAt(i);
        }
        
        while (Points.Count < Count) {
            Points.Add(new(new(
                new Random().Next(0, 100) / 100f,
                0
            ), new Random().Next(10, 20) / 100f));
        }
    }
    
    protected override void OnLoad() {
        base.OnLoad();
        
        for (var i = 0; i < Count; i++) {
            Points.Add(new(new(
                new Random().Next(0, 100) / 100f,
                new Random().Next(0, 100) / 100f
            ), new Random().Next(10, 20) / 100f));
        }
    }
}

public class MyApp : Application {
    private readonly BoxElement _box3 = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            Size = new(100)
        },
        Color = new(128, 128, 128, 50),
        ZIndex = 5,
        MaskMouseEvents = false
    };
    
    private readonly BoxElement _box4 = new() {
        Transform = new() {
            ParentScale = Vector2.One
        },
        Color = SKColors.Gray,
        MaskMouseEvents = true
    };
    
    private readonly DraggableElement _box4Wrapper = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            RotationOffset = new(0.5f),
            Size = new(50, 20)
        },
        ZIndex = 6
    };
    
    private readonly List<double> _fps = new();
    
    private readonly TextElement _fpsText = new() {
        Transform = new() {
            OffsetPosition = new(1, 0),
            AnchorPosition = new(1, 0),
            LocalPosition = new(-10, 10)
        },
        Text = "FPS: 0",
        TextSize = 30,
        ZIndex = 1000,
        Color = SKColors.Black
    };
    
    private readonly SnowflakeOverlay _overlay = new() {
        Transform = new() {
            ScaleHeight = true,
            ScaleWidth = true
        },
        ZIndex = 10,
        Count = 50,
        Scale = 5,
        Color = new(128, 128, 128, 50),
        MouseInteraction = false
    };
    private readonly SKPaint _paint = new() {
        Color = SKColors.White,
        TextSize = 30
    };
    
    private readonly BoxElement _wrapper = new() {
        Transform = new() {
            ParentScale = new(1f)
        },
        Color = new(10, 30, 50),
        MaskMouseEvents = true
    };
    
    private SKRuntimeEffect? _effect;
    private SKShader? _shader;
    
    protected override void OnLoad() {
        base.OnLoad();
        _wrapper.AddChild(_box3);
        _box4Wrapper.AddChild(_box4);
        _wrapper.AddChild(_box4Wrapper);
        Element.AddChild(_wrapper);
        _wrapper.AddChild(_overlay);
        
        _box4Wrapper.MaskMouseEvents = false;
        
        _box4Wrapper.DoMouseDown += x => {
            if (x != MouseButton.Left) return;
            _box4.Color = SKColors.White;
            // box4Wrapper.Transform.LocalRotation += 45;
        };
        
        _box4Wrapper.DoMouseUp += x => {
            if (x != MouseButton.Left) return;
            _box4.Color = SKColors.Gray;
        };
        
        _effect = SKRuntimeEffect.Create(
            Resources.LoadYoruResourceFileS("shaders/vignette.sksl"),
            out var errors
        );
        
        
        if (errors != null) {
            Console.WriteLine(errors);
        }
    }
    
    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.G)
            _wrapper.MouseInteraction = !_wrapper.MouseInteraction;
        else if (key == Key.X)
            DrawingMethod = Enumerated.Next(DrawingMethod);
        else if (key == Key.U)
            Handler.UpdateFrequency += 10;
        else if (key == Key.J)
            Handler.UpdateFrequency -= 10;
        else if (key == Key.R)
            Handler.UpdateFrequency = 60;
    }
    
    protected override void OnRender() {
        base.OnRender();
        
        _fps.Add(1.0 / RenderTime.DeltaTime);
        var avg = (int)_fps.Average();
        
        if (_fps.Count > 10) _fps.RemoveAt(0);
        
        AppCanvas.DrawText("FPS: " + avg, 10, 40, _paint);
        AppCanvas.DrawRect(0, 0, Size.X, Size.Y, new() {
            Shader = _shader
        });
        
        _fpsText.Text = "FPS: " + avg;
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        // make box rotate towards center
        var angle = (float)Math.Atan2(
            Size.Y / 2 - _box4Wrapper.Transform.WorldPosition.Y,
            Size.X / 2 - _box4Wrapper.Transform.WorldPosition.X
        );
        
        _box4Wrapper.Transform.LocalRotation = angle * 180 / MathF.PI;
    }
    
    protected override void OnResize(int width, int height) {
        base.OnResize(width, height);
        SKRuntimeEffectUniforms s = new(_effect);
        s["res"] = new[] { width / CanvasScale.X, height / CanvasScale.Y };
        s["power"] = 10f;
        s["extend"] = 0.2f;
        s["color"] = new float[] { 0, 0, 0 };
        _shader = _effect?.ToShader(true, s);
        
        
        _overlay.Scale = Math.Max(Math.Max(width, height) / 300f, 1);
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GlWindow {
            App = new MyApp(),
            Title = "My Application"
        }.Run();
    }
}
