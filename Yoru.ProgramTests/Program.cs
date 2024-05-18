using System.Diagnostics;
using System.Numerics;
using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Utilities;
using Yoru.Platforms.GL;
using System.Reflection;
using System.Text;

public class MyApp : Application {
    private readonly BoxElement box3 = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            Size = new(100)
        },
        Color = new SKColor(255, 0, 0, 50),
        ZIndex = 5,
        MaskMouseEvents = false
    };
    
    private readonly BoxElement box4 = new() {
        Transform = new() {
            ParentScale = Vector2.One
        },
        Color = SKColors.Green,
        MaskMouseEvents = true
    };

    private readonly BoxElement wrapper = new() {
        Transform = new() {
            ParentScale = new(1f)
        },
        Color = SKColors.White,
        MaskMouseEvents = true
    };

    private readonly DraggableElement box4Wrapper = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            Size = new(50)
        },
        ZIndex = 6
    };

    public PathElement path = new() {
        Color = SKColors.Red,
        StrokeWidth = 5,
        Transform = new() {
            OffsetPosition = new(1),
            AnchorPosition = new(1)
        },
        Style = SKPaintStyle.Stroke
    };

    SKRuntimeEffect? effect;
    SKRuntimeEffectUniforms? uniforms;
    SKShader? shader;
    
    protected override void OnLoad() {
        base.OnLoad();
        wrapper.AddChild(box3);
        box4Wrapper.AddChild(box4);
        wrapper.AddChild(box4Wrapper);
        wrapper.AddChild(path);
        Element.AddChild(wrapper);
        
        box4Wrapper.MaskMouseEvents = false;
        
        box4Wrapper.DoMouseDown += (_) => box4.Color = SKColors.Yellow;
        box4Wrapper.DoMouseUp += (_) => box4.Color = SKColors.Green;

        effect = Resources.LoadEffect("Shaders/stripe.sksl");

        if (effect == null) {
            Console.WriteLine("Failed to load effect");
            Close();
            return;
        }

        uniforms = new(effect);
        uniforms["stripeColor"] = new float[] { 1.0f, 0.0f, 1.0f, 0.1f };
        uniforms["bgColor"] = new float[] { 0.0f, 0.0f, 1.0f, 0.5f };
        uniforms["speed"] = 40f;
        uniforms["stripeWidth"] = 30f;
        uniforms["stripeGap"] = 30f;

        Animations.Start(new Animation() {
            Duration = 1,
            LoopMode = AnimationLoopMode.PingPong,
            Easing = Easing.BounceOut,
            OnUpdate = (t) => {
                box3.Transform.Size = new((float)t * 100 + 50);
                box3.Transform.LocalRotation = (float)t * 360;
            }
        });
    }
    
    bool direction = false;
    protected override void OnUpdate() {
        base.OnUpdate();

        uniforms!["time"] = (float)UpdateTime.Time;
        shader = effect?.ToShader(true, uniforms);
    }

    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.G)
            wrapper.MouseInteraction = !wrapper.MouseInteraction;
        else if (key == Key.X)
            DrawingMethod = Enumerated.Next<ElementDrawingMethod>(DrawingMethod);
        else if (key == Key.J)
            Handler.RenderFrequency = Handler.RenderFrequency == 60 ? 1 : 60;
    }

    List<double> fps = new();
    SKPaint paint = new SKPaint {
        Color = SKColors.Gray,
        TextSize = 30
    };

    float xPos = 0;
    List<Vector2> Points = new();
    protected override void OnRender() {
        base.OnRender();

        fps.Add(1.0 / RenderTime.DeltaTime);
        int avg = (int)fps.Average();

        Points.Add(new Vector2(xPos, (avg / 300) * 100));
        path.Points = Points.ToArray();
        xPos += 1;
        if (fps.Count > 10) {
            Points.RemoveAt(0);
            fps.RemoveAt(0);
        }

        AppCanvas.DrawText("FPS: " + avg, 10, 40, paint);
        AppCanvas.DrawText("FRAME: " + Debugging.FrameCount, 10, 80, paint);
        AppCanvas.DrawRect(0, 0, Size.X, Size.Y, new SKPaint {
            Shader = shader
        });
    }

    protected override void OnResize(int width, int height) {
        base.OnResize(width, height);
        uniforms!["res"] = new float[] { width / CanvasScale, height / CanvasScale };
        shader = effect?.ToShader(true, uniforms);
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GLWindow {
            App = new MyApp()
        }.Run();
    }
}
