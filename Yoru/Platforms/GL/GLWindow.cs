#nullable disable

using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Yoru.Input;
using OGL = OpenTK.Graphics.OpenGL4.GL;

namespace Yoru.Platforms.GL;

public class GLWindow : GameWindow, IApplicationHandler {
    private Vector2 _dpi = Vector2.One;
    public Application App;
    public GLRenderer Renderer;
    public GLWindow() : base(new() { UpdateFrequency = 300 }, new() { Title = "Yoru App", Flags = ContextFlags.ForwardCompatible }) { }
    public GLWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }
    
    public new double RenderFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    public new double UpdateFrequency { get => base.UpdateFrequency; set => base.UpdateFrequency = value; }
    
    public new string Title { get => base.Title; set => base.Title = value; } // Only for bridging
    public new Vector2 Size { get => new(FramebufferSize.X, FramebufferSize.Y); }
    
    public Vector2 GetDPI() {
        var val = TryGetCurrentMonitorScale(out var horizontal, out var vertical);
        if (!val) return _dpi = Vector2.One;
        _dpi = new(horizontal, vertical);
        return _dpi;
    }
    
    protected override void OnLoad() {
        base.OnLoad();
        Renderer = new();
        
        OGL.Enable(EnableCap.FramebufferSrgb);
        OGL.Enable(EnableCap.Multisample);
        OGL.Enable(EnableCap.DepthTest);
        OGL.Enable(EnableCap.Blend);
        OGL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        OGL.DepthFunc(DepthFunction.Less);
        OGL.ClearColor(0, 0, 0, 1);
        
        App.Handler = this;
        App.Renderer = Renderer;
        Renderer.GLContext = Context;
        
        App.CanvasScale = GetDPI();
        App.Load();
    }
    
    protected override void OnRenderFrame(FrameEventArgs args) => App.Render();
    protected override void OnUpdateFrame(FrameEventArgs args) => App.Update();
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) => App.Resize(FramebufferSize.X, FramebufferSize.Y, GetDPI());
    
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
    protected override void OnMouseMove(MouseMoveEventArgs e) =>
        App.MouseMove(
            Vector2.Clamp(new(MouseState.Position.X, MouseState.Position.Y), Vector2.Zero, new(ClientSize.X, ClientSize.Y)) * (_dpi / App.CanvasScale)
        );
}
