#nullable disable

using System.Numerics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Yoru.Input;

namespace Yoru.Platforms.GL;

public class GLWindow : GameWindow, IApplicationHandler {
    private float _dpi = 1;
    public Application App;
    public GLRenderer Renderer;
    public GLWindow() : base(new GameWindowSettings() { UpdateFrequency = 300 }, new NativeWindowSettings() { Title = "Yoru App" }) { }
    public GLWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }
    
    public new double RenderFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    public new double UpdateFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    
    public new string Title { get => base.Title; set => base.Title = value; } // Only for bridging
    public new Vector2 Size { get => new(FramebufferSize.X, FramebufferSize.Y); }
    
    protected override void OnLoad() {
        base.OnLoad();
        Renderer = new();
        
        App.Handler = this;
        App.Renderer = Renderer;
        Renderer.GLContext = Context;
        
        _dpi = FramebufferSize.X / base.Size.X;
        App.CanvasScale = _dpi;
        App.Load();
    }
    
    protected override void OnRenderFrame(FrameEventArgs args) => App.Render();
    protected override void OnUpdateFrame(FrameEventArgs args) => App.Update();
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        _dpi = FramebufferSize.X / base.Size.X;
        App.Resize(FramebufferSize.X, FramebufferSize.Y, _dpi);
    }
    
    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        if (e.IsRepeat) return;
        App.KeyDown((Key)e.Key);
    }
    
    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        if (e.IsRepeat) return;
        App.KeyUp((Key)e.Key);
    }
    
    protected override void OnMouseDown(MouseButtonEventArgs e) => App.MouseDown((MouseButton)e.Button);
    protected override void OnMouseUp(MouseButtonEventArgs e) => App.MouseUp((MouseButton)e.Button);
    protected override void OnMouseMove(MouseMoveEventArgs e)
        => App.MouseMove(Vector2.Clamp(new(MouseState.Position.X / App.CanvasScale * _dpi, MouseState.Position.Y / App.CanvasScale * _dpi), Vector2.Zero, Size));
}
