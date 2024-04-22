#nullable disable

using System.Numerics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Yoru.Input;

namespace Yoru.Platforms.GL;

public class GLWindow : GameWindow, IApplicationHandler {
    public GLWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default) { }
    public GLWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    public Application app;
    public GLRenderer renderer;

    protected override void OnLoad() {
        base.OnLoad();

        renderer = new();

        app.Handler = this;
        app.Renderer = renderer;
        renderer.GLContext = this.Context;
        
        app.Load();
    }

    protected override void OnRenderFrame(FrameEventArgs args) => app.Render();
    protected override void OnUpdateFrame(FrameEventArgs args) => app.Update();
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) => app.Resize(e.Width, e.Height);

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        if (e.IsRepeat) return;
        app.KeyDown((Key)e.Key);
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        if (e.IsRepeat) return;
        app.KeyUp((Key)e.Key);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) => app.MouseDown((MouseButton)e.Button);
    protected override void OnMouseUp(MouseButtonEventArgs e) => app.MouseUp((MouseButton)e.Button);
    protected override void OnMouseMove(MouseMoveEventArgs e) => app.MouseMove(Vector2.Clamp(new(MouseState.Position.X, MouseState.Position.Y), Vector2.Zero, Size));

    public new double RenderFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    public new double UpdateFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }

    public void SetTitle(string title) => Title = title;
    public void SetSize(int width, int height) => base.Size = new(width, height);
    public new Vector2 Size { get => new Vector2(base.FramebufferSize.X, base.FramebufferSize.Y); }
}
