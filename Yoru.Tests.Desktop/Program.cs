using System.Numerics;
using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;
using Yoru.Platforms.SDL;

public class Snowflake : CircleElement {
    public float Speed = 300;

    protected override void OnLoad() {
        base.OnLoad();
        Transform.LocalPosition = new Vector2(new Random().Next(0, (int)App.Size.X), new Random().Next(0, (int)App.Size.Y));
    }

    protected override void OnUpdate() {
        base.OnUpdate();

        Transform.LocalPosition += new Vector2(0, Speed * (float)App.UpdateTime.DeltaTime);

        if (Transform.WorldPosition.Y - Transform.Size.Y > App.Size.Y)
            Transform.LocalPosition = new Vector2(new Random().Next(0, (int)App.Size.X), -Transform.Size.Y);
    }
}

public class SnowflakeOverlay : ColorableElement {
    public List<Tuple<Vector2, float>> Points = new(); // they are from 0 to 1
    public int Count = 100;
    public float Scale = 1f;
    
    protected override void OnRender(SKCanvas canvas) {
        for (int i = 0; i < Points.Count; i++) {
            Vector2 pos = (Points[i].Item1 * new Vector2(Transform.Size.X, Transform.Size.Y + Scale * 2)) - new Vector2(0, Scale);
            canvas.DrawCircle(new(pos.X, pos.Y), Scale, Paint);

            Points[i] = new(Points[i].Item1 + new Vector2(0, Points[i].Item2) * (float)App.UpdateTime.DeltaTime, Points[i].Item2);
            if (Points[i].Item1.Y > 1) Points.RemoveAt(i);
        }

        while (Points.Count < Count) {
            Points.Add(new(new Vector2(
                new Random().Next(0, 100) / 100f,
                0
            ), new Random().Next(10, 20) / 100f));
        }
    }

    protected override void OnLoad() {
        base.OnLoad();

        for (int i = 0; i < Count; i++) {
            Points.Add(new(new Vector2(
                new Random().Next(0, 100) / 100f,
                new Random().Next(0, 100) / 100f
            ), new Random().Next(10, 20) / 100f));
        }
    }
}

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
            Size = new(50),
        },
        ZIndex = 6
    };

    private readonly TextElement fpsText = new() {
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

    SnowflakeOverlay overlay = new() {
        Transform = new() {
            ScaleHeight = true,
            ScaleWidth = true
        },
        ZIndex = 10,
        Count = 50,
        Scale = 5,
        Color = new SKColor(128, 128, 128, 50),
        MouseInteraction = false
    };

    SKRuntimeEffect? effect;
    SKRuntimeEffectUniforms? uniforms;
    SKShader? shader;
    
    protected override void OnLoad() {
        base.OnLoad();
        wrapper.AddChild(box3);
        box4Wrapper.AddChild(box4);
        wrapper.AddChild(box4Wrapper);
        Element.AddChild(wrapper);
        wrapper.AddChild(overlay);
        Element.AddChild(fpsText);
        
        box4Wrapper.MaskMouseEvents = false;
        
        box4Wrapper.DoMouseDown += (x) => {
            if (x != MouseButton.Left) return;
            box4.Color = SKColors.Yellow;
        };
        box4Wrapper.DoMouseUp += (x) => {
            if (x != MouseButton.Left) return;
            box4.Color = SKColors.Green;
        };

        effect = SKRuntimeEffect.Create(
            """
            uniform vec2 res;
            uniform float softness;

            float distance(vec2 a, vec2 b) {
                return sqrt(pow(a.x - b.x, 2.0) + pow(a.y - b.y, 2.0));
            }

            float smoothstep(float edge0, float edge1, float x) {
                float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
                return t * t * (3.0 - 2.0 * t);
            }

            half4 main(vec2 fragCoord) {
                vec2 center = res / 2.0;
                
                float distX = abs(fragCoord.x - center.x) / (res.x / 2.0);
                float distY = abs(fragCoord.y - center.y) / (res.y / 2.0);
                
                float vignette = 1.0 - (max(1.0 - distX * distX / softness, 0.0) * max(1.0 - distY * distY / softness, 0.0));
                vignette = smoothstep(0.0, 1.0, vignette); 
                
                return half4(0.0, 0.0, 0.0, vignette);
            }
            """,

            out string errors
        );


        if (errors != null) {
            Console.WriteLine(errors);
            return;
        }
    }
    
    bool direction = false;
    protected override void OnUpdate() {
        base.OnUpdate();
        if (!box4Wrapper.IsMouseDown)
            box4Wrapper.Transform.LocalPosition += (direction ? 1 : -1) * new Vector2((float)Math.Sin(UpdateTime.DeltaTime) * 500f, 0);
        if (box4Wrapper.Transform.WorldPosition.X > Size.X - 50 || box4Wrapper.Transform.WorldPosition.X < 0) {
            direction = !direction;
            box4Wrapper.Transform.WorldPosition = new Vector2(Math.Clamp(box4Wrapper.Transform.WorldPosition.X, 0, Size.X - 50), box4Wrapper.Transform.WorldPosition.Y);
        }
    }

    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.G)
            wrapper.MouseInteraction = !wrapper.MouseInteraction;
        else if (key == Key.X)
            DrawingMethod = Enumerated.Next<ElementDrawingMethod>(DrawingMethod);
        else if (key == Key.U)
            Handler.UpdateFrequency += 10;
        else if (key == Key.J)
            Handler.UpdateFrequency -= 10;
        else if (key == Key.R)
            Handler.UpdateFrequency = 60;
    }

    List<double> fps = new();
    SKPaint paint = new SKPaint {
        Color = SKColors.White,
        TextSize = 30
    };

    protected override void OnRender() {
        base.OnRender();

        fps.Add(1.0 / RenderTime.DeltaTime);
        int avg = (int)fps.Average();

        if (fps.Count > 10) fps.RemoveAt(0);

        AppCanvas.DrawText("FPS: " + avg, 10, 40, paint);
        AppCanvas.DrawRect(0, 0, Size.X, Size.Y, new SKPaint {
            Shader = shader
        });

        fpsText.Text = "FPS: " + avg;
    }

    protected override void OnResize(int width, int height) {
        base.OnResize(width, height);
        SKRuntimeEffectUniforms s = new(effect);
        s["res"] = new float[] { width, height };
        s["softness"] = 2f;
        shader = effect?.ToShader(true, s);


        overlay.Scale = Math.Max(Math.Max(width, height) / 300f, 1);
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GLWindow() {
            App = new MyApp(),
            Title = "My Application"
        }.Run();
    }
}
