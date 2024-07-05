using System.Diagnostics;
using System.Numerics;
using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms.GL;
using Yoru.Platforms.SDL;
using Yoru.Utilities;

public class TestApp : Application {
    public List<BoxElement> Boxes = new();

    protected override void OnLoad() {
        base.OnLoad();

        Element parent = Element;

        for (int j = 0; j < 200; j++) {
            SKColor color = new SKColor[] {
                // pastel red, green, blue, yellow
                new(255, 105, 97, 100),
                new(119, 221, 119, 100),
                new(119, 158, 203, 100),
                new(255, 255, 102, 100)
            }[j % 3];

            BoxElement elm = new BoxElement {
                Transform = new() {
                    LocalPosition = new(0),
                    AnchorPosition = new(0.5f),
                    OffsetPosition = new(0.5f),
                    // goes from 1 to 100
                    Size = new(j * 4),
                    RotationOffset = new(j / 200)
                },
                ZIndex = 200 - j,
                Color = color
            };

            elm.DoMouseEnter += () => elm.Color = SKColors.White;
            elm.DoMouseLeave += () => elm.Color = color;
            
            Element.AddChild(elm);
            parent = elm;

            Boxes.Add(elm);
        }
    }

    protected override void OnKeyDown(Key key) {
        base.OnKeyDown(key);
        if (key == Key.X) {
            DrawingMethod = Enumerated.Next(DrawingMethod);
        }
    }

    protected override void OnRender() {
        base.OnRender();
        AppCanvas.DrawText("Render FPS: " + (1.0 / RenderTime.DeltaTime).ToString("0.00"), new(10, 30), new SKPaint {
            Color = SKColors.White,
            TextSize = 20
        });

        AppCanvas.DrawText("Update FPS: " + (1.0 / UpdateTime.DeltaTime).ToString("0.00"), new(10, 60), new SKPaint {
            Color = SKColors.White,
            TextSize = 20
        });
    }

    protected override void OnUpdate() {
        base.OnUpdate();
        
        float val = 20;
        foreach (Element elm in Boxes) {
            val -= 0.02f;
            elm.Transform.LocalRotation += (float)(UpdateTime.DeltaTime * val);
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GlWindow() {
            App = new TestApp(),
            VSync = false,
        }.Run();
    }
}
