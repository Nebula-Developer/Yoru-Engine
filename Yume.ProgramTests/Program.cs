#pragma warning disable CS0162

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using Yume.Graphics.Elements;
using Yume.Input;
using Yume.Windowing;
using Window = Yume.Windowing.Window;

namespace Yume;

public class InheritWindow : Window {
    private readonly List<Box> Boxes = new();
    private readonly FlexBox Flex = new();
    private readonly ClipMask mask = new();
    private bool b;

    private Vector2 pos = new(0);

    protected override void Load() {
        Flex.Direction = FlexDirection.Row;
        Flex.Margin = 5;

        Flex.Parent = mask;
        mask.Parent = Element;

        mask.Transform.Size = new Vector2(500);

        Box box2 = new();
        box2.Parent = mask;
        box2.Transform.ScaleWidth = true;
        box2.Transform.ScaleHeight = true;
        box2.Color = SKColors.Red;
        box2.ZIndex = -1000;

        for (var i = 0; i < 10; i++) {
            Box box = new();
            box.Transform.Size = new Vector2(100, new Random().Next(100, 300));
            box.Color = SKColors.Orange;
            Boxes.Add(box);
            Flex.AddChild(box);
        }
    }

    protected override void Render() {
        base.Render();

        Canvas.DrawText("RFPS: " + (1.0 / RenderTime.DeltaTime).ToString("0.00"), new SKPoint(10, 20), new SKPaint {
            Color = SKColors.White,
            TextSize = 20
        });

        Canvas.DrawText("UFPS: " + (1.0 / UpdateTime.DeltaTime).ToString("0.00"), new SKPoint(10, 40), new SKPaint {
            Color = SKColors.White,
            TextSize = 20
        });
    }

    protected override void Update() {
        base.Update();


        Flex.Transform.LocalPosition = Vector2.Lerp(
            Flex.Transform.LocalPosition,
            pos,
            10f * (float)UpdateTime.DeltaTime);

        if (KeyboardState.IsKeyDown(Keys.Down))
            Boxes[0].Transform.Size += new Vector2(0, 200 * (float)UpdateTime.DeltaTime);

        if (KeyboardState.IsKeyDown(Keys.Up))
            Boxes[0].Transform.Size -= new Vector2(0, 200 * (float)UpdateTime.DeltaTime);

        if (KeyboardState.IsKeyDown(Keys.Left)) Flex.Margin += 200 * (float)UpdateTime.DeltaTime;

        if (KeyboardState.IsKeyDown(Keys.Right)) Flex.Margin -= 200 * (float)UpdateTime.DeltaTime;

        if (Input.GetKeyDown(KeyCode.E))
            Flex.Direction = Flex.Direction == FlexDirection.Row ? FlexDirection.Column : FlexDirection.Row;

        if (Input.GetKey(KeyCode.O)) mask.Transform.Size += new Vector2(300 * (float)UpdateTime.DeltaTime);

        if (Input.GetKey(KeyCode.I)) mask.Transform.Size -= new Vector2(300 * (float)UpdateTime.DeltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine(IsMultiThreaded);
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            var pos = mask.Transform.WorldPosition;
            var a = b;
            b = !b;
            var start = DateTime.Now;
            Animations.Add(new Animation {
                Duration = 0.5f,
                LoopMode = AnimationLoopMode.None,
                Delay = 0f,
                OnUpdate = t => {
                    mask.Transform.WorldPosition = Vector2.Lerp(pos, a ? new Vector2(100) : new Vector2(0), (float)t);
                    Boxes[0].Color = new SKColor(
                        (byte)(255 - 255 * t),
                        (byte)(255 * t),
                        0);
                },
                OnComplete = () => {
                    Console.WriteLine("Complete: took " + (DateTime.Now - start).TotalMilliseconds + "ms");
                },
                // cubic out
                Easing = t => 1 - Math.Pow(1 - t, 3)
            }, "anim");
            Console.WriteLine(a);
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        base.OnMouseWheel(e);
        pos += new Vector2(0, e.OffsetY * 10);
    }
}

public static class Program {
    public static void Main(string[] args) {
        InheritWindow window = new() {
            UpdateFrequency = 144,
            RenderFrequency = 144
        };

        window.Run();
    }
}