#nullable disable

using System.Numerics;
using OpenTK.Graphics.OpenGL;
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

    private SKRuntimeEffect effect;
    private SKShader shader;
    private SKPaint paint;

    protected override void OnLoad() {
        Element.AddChild(pointA);
        Element.AddChild(pointB);
        Element.AddChild(pointC);
        Element.AddChild(path);
        Element.AddChild(circTest);

        string glslCode = @"
            uniform float time;
            uniform float2 resolution;

            float atan2(float y, float x) {
                float t0 = max(abs(x), abs(y));
                float t1 = min(abs(x), abs(y));
                float t3 = t1 / t0;
                float t4 = t3 * t3;
                float t7 = 0.002823638962581753073;
                t7 = (-0.0159569028764963159 + t7 * t4) * t4;
                t7 = (0.0742610645340762951 + t7 * t4) * t4;
                t7 = (-0.212114803876518898 + t7 * t4) * t4;
                t7 = (1.5707288 + t7 * t4) * t3;
                return x < 0 ? 3.141592653589793238 - t7 : t7;
            }

            float lerp(float a, float b, float t) {
                return a + (b - a) * t;
            }

            float vecDist(float2 a, float2 b) {
                return sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
            }
            
            half4 main(float2 p) {
                // get distance from center
                float2 uv = p / resolution;
                float2 center = float2(0.5, 0.5);
                float dist = vecDist(uv, center) / length(center);
                float angle = atan2(uv.y - center.y, uv.x - center.x);
                float t = time * 0.5;

                // get color based on distance
                float r = lerp(1, 0, dist);
                float g = lerp(1, 0, dist);
                float b = lerp(1, 0, dist);

                // get color based on angle
                r = lerp(r, 1, abs(cos(angle + t)));
                g = lerp(g, 0, abs(tan(angle + t * 0.5)));
                b = lerp(b, 1, abs(tan(angle + t)));

                // vignette
                float a = length(uv - center) / length(center);

                return half4(r, g, b, a);

                // return dist < 0.5 ? half4(1, 0, 0, 1) : half4(0, 1, 0, 1);
            }
        ";

        effect = SKRuntimeEffect.Create(glslCode, out string err);
        
        if (err != null) {
            Console.WriteLine(err);
            return;
        }

        Random instance = new();
        CanvasIsolationElement isolation = new();
        for (int i = 0; i < 100; i++)
        {
            DraggableElement hold = new() {
                Transform = new() {
                    LocalPosition = new(instance.Next(0, 1000), instance.Next(0, 1000)),
                    Size = new(100)
                }
            };

            BoxElement box = new() {
                Transform = new() {
                    ScaleHeight = true,
                    ScaleWidth = true

                }
            };

            box.Color = new((byte)instance.Next(0, 255), (byte)instance.Next(0, 100), 255);
            hold.AddChild(box);
            Element.AddChild(hold);
        }

        void Update() {
            List<Vector2> points = new();
            var a = pointA.Transform.WorldPosition;
            var b = pointB.Transform.WorldPosition;
            var c = pointC.Transform.WorldPosition;
            points.Add(a);

            for (float t = 0; t <= 1; t += 0.01f)
            {
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
        if (Input.GetKeyDown(Key.Space))
        {
            DrawingMethod = Enumerated.Next(DrawingMethod);
        }
    }

    SKRuntimeEffectUniforms uniforms;
    protected override void OnRender() {
        base.OnRender();
        if (uniforms == null) uniforms = new(effect);
        uniforms["time"] = (float)RenderTime.Time;
        uniforms["resolution"] = new float[] { FramebufferSize.X, FramebufferSize.Y };
        shader = effect.ToShader(true, uniforms);
        // get blend mode based on time, so it alternates
        // SKBlendMode mode = Enumerated.Index<SKBlendMode>((int)(RenderTime.Time % Enumerated.Count<SKBlendMode>()));

        paint = new() {
            IsAntialias = true,
            Shader = shader,
            // apply tinge with color filter
            // ColorFilter = SKColorFilter.CreateBlendMode(new SKColor((byte)((RenderTime.Time % 5 / 5) * 255), 100, 50, 100), mode),
            // Color = SKColors.Transparent,
        };

        AppCanvas.DrawRect(0, 0, Size.X, Size.Y, paint);

        float fps = 1 / (float)RenderTime.DeltaTime;
        AppCanvas.DrawText($"FPS: {fps:0.00}", 10, 30, new() {
            Color = SKColors.White,
            TextSize = 20
        });
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
