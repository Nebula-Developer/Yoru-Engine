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
    private bool b;
    private readonly List<Box> Boxes = new();
    private readonly FlexBox Flex = new();
    private readonly ClipMask mask = new();

    private Vector2 pos = new(0);

    protected override void Load() {
        Flex.Direction = FlexDirection.Column;

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

        if (KeyboardState.IsKeyDown(Keys.R))
            Boxes[0].Transform.WorldPosition -= new Vector2(0, 500 * (float)UpdateTime.DeltaTime);

        if (KeyboardState.IsKeyPressed(Keys.X)) Console.WriteLine("X");
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

        if (Input.GetKeyDown(KeyCode.C)) {
            var pos = mask.Transform.WorldPosition;
            var a = b;
            b = !b;
            Animations.Add(new Animation {
                Duration = 1,
                LoopMode = AnimationLoopMode.Forward,
                OnUpdate = t => {
                    mask.Transform.WorldPosition = Vector2.Lerp(pos, a ? new Vector2(100) : new Vector2(0), (float)t);
                }
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