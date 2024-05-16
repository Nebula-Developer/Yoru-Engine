using SkiaSharp;
using Yoru;
using Yoru.Elements;
using Yoru.Platforms.GL;

public class MyApp : Application {
    private readonly BoxElement box = new() {
        Color = SKColors.Green,
        Transform = new() {
            Size = new(100),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            RotationOffset = new(0.5f)
        }
    };

    private BoxElement box3 = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            Size = new(100)
        },
        Color = SKColors.Red,
        ZIndex = 5
    };

    private BoxElement box4 = new() {
        Transform = new() {
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            Size = new(50)
        },
        Color = SKColors.Green,
        ZIndex = 6
    };
    
    protected override void OnLoad() {
        base.OnLoad();
        Element.AddChild(box);
        Element.AddChild(box3);
        Element.AddChild(box4);

        box3.DoMouseEnter += () => box3.Color = SKColors.Blue;
        box3.DoMouseLeave += () => box3.Color = SKColors.Red;

        box4.MaskMouseEvents = false;

        box4.DoMouseEnter += () => box4.Color = SKColors.Yellow;
        box4.DoMouseLeave += () => box4.Color = SKColors.Green;
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        box.Transform.LocalRotation += (float)UpdateTime.DeltaTime * 100f;
        box4.Transform.LocalPosition = new(100 * (float)Math.Sin(UpdateTime.Time), 0);
    }
}

public static class Program {
    public static void Main(string[] args) {
        new GLWindow {
            App = new MyApp()
        }.Run();
    }
}
