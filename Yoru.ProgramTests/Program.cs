#nullable disable

using System.Reflection;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public class TestAppSwitcher : Application {
    public EasingTestApp easing = new();
    public ImageElement backgroundImage = new(AppDomain.CurrentDomain.BaseDirectory + "Image.jpg");
    private float imageRatio;

    public void ResizeBackground() {
        float width = Handler.Size.X;
        float height = Handler.Size.Y;
        if (width / height > imageRatio) {
            backgroundImage.Transform.Size = new(width, width / imageRatio);
        } else {
            backgroundImage.Transform.Size = new(height * imageRatio, height);
        }
    }

    public override void OnLoad() {
        base.OnLoad();
        easing.Renderer = this.Renderer;
        easing.Handler = this.Handler;
        easing.FlushRenderer = false;
        easing.ClearRenderer = false;
        easing.Load();

        Element.AddChild(backgroundImage);
        backgroundImage.Transform.OffsetPosition = new(0.5f);
        backgroundImage.Transform.AnchorPosition = new(0.5f);
        imageRatio = backgroundImage.Transform.Size.X / backgroundImage.Transform.Size.Y;
        ResizeBackground();
    }

    public override void OnRender() {
        base.OnRender();
        easing.Render();
    }

    public override void OnUpdate() {
        base.OnUpdate();
        easing.Update();
    }

    public override void OnResize(int width, int height) {
        base.OnResize(width, height);
        easing.Resize(width, height);
        ResizeBackground();
    }
}


public static class Program {
    public static void Main(string[] args) {
        GLWindow programTestWindow = new();
        Console.CancelKeyPress += (s, e) => { programTestWindow.Close(); e.Cancel = true; };
        programTestWindow.app = new TestAppSwitcher();
        programTestWindow.Run();
        Console.WriteLine("Program killed");
        programTestWindow.Dispose();
    }
}
