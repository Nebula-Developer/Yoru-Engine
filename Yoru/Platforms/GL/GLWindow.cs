#nullable disable

using System.Numerics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using Yoru.Input;

namespace Yoru.Platforms.GL;

public class GLWindow : GameWindow, IApplicationHandler {
    public Application app;
    public GLRenderer renderer;
    public GLWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default) { }
    public GLWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }
    
    public new double RenderFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    public new double UpdateFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    
    public new string Title { get => base.Title; set => base.Title = value; } // Only for bridging
    public new Vector2 Size { get => new(FramebufferSize.X, FramebufferSize.Y); }
    
    protected override void OnLoad() {
        base.OnLoad();
        
        UpdateFrequency = 200;
        renderer = new();
        
        app.Handler = this;
        app.Renderer = renderer;
        renderer.GLContext = Context;
        
        app.Load();
        // hide cursor
        Cursor = MouseCursor.Empty;
    }
    
    protected override void OnRenderFrame(FrameEventArgs args) => app.Render();
    protected override void OnUpdateFrame(FrameEventArgs args) => app.Update();
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        float dpi = FramebufferSize.X / base.Size.X;
        app.CanvasScale = dpi;
        app.Resize(e.Width, e.Height);
    }
    
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
}
