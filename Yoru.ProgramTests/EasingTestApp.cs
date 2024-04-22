
using System.Reflection;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Mathematics;

namespace Yoru.ProgramTests;

public class EasingTestApp : Application {
    BoxElement box = new() {
        Color = SKColors.White,
        Transform = new() {
            Size = new(300),
            AnchorPosition = new(0.5f),
            OffsetPosition = new(0.5f),
            RotationOffset = new(0.5f)
        }
    };

    TextElement methodName = new() {
        AutoResize = false,
        Transform = new() {
            Size = new(50),
            ScaleWidth = true
        },
        TextSize = 40,
        Alignment = TextAlignment.Center,
        Color = SKColors.Red
    };

    BoxElement progress = new() {
        Color = new SKColor(100, 100, 100),
        Transform = new() {
            Size = new(50)
        }
    };

    public override void OnLoad() {
        base.OnLoad();
        Element.AddChild(box);
        Element.AddChild(progress);
        Element.AddChild(methodName);

        int e = 0;
        MethodInfo[] methods = typeof(Easing).GetMethods(BindingFlags.Public | BindingFlags.Static);
        Func<double, double> ease = (Func<double, double>)methods[0].CreateDelegate(typeof(Func<double, double>));
        methodName.Text = methods[0].Name;

        Animations.Add(new Animation() {
            Duration = 5,
            LoopMode = AnimationLoopMode.Forward,
            OnUpdate = (double t) => {
                box.Transform.LocalRotation = (float)ease(t) * 360;
                progress.Transform.Size = new(Element.Transform.Size.X * (float)t, 50);
            },
            OnLoop = (double t) => {
                e++;
                ease = (Func<double, double>)methods[e % methods.Length].CreateDelegate(typeof(Func<double, double>));
                methodName.Text = methods[e % methods.Length].Name;
            }
        });
    }
}
