#nullable disable

using SkiaSharp;
using Yoru.Graphics;
using Yoru.Platforms;
using Yoru.Input;
using System.Numerics;

namespace Yoru;

public class Application {
    public IApplicationHandler Handler { get; set; } = new HeadlessHandler();
    
    public bool FlushRenderer { get; set; } = true;
    public bool ClearRenderer { get; set; } = true;
    public Renderer Renderer { get; set; } = new EmptyRenderer();
    public SKCanvas Canvas => Renderer.Canvas;

    public TimeContext UpdateTime { get; protected set; }
    public TimeContext RenderTime { get; protected set; }

    public InputContext Input { get; protected set; }
    public AnimationContext Animations { get; protected set; }

    public RootElement Element {
        get {
            if (_element == null) _element = new(this);
            return _element;
        }
    }
    private RootElement _element;

    public virtual void OnLoad() { }
    public virtual void OnUpdate() { }
    public virtual void OnRender() { }
    public virtual void OnResize(int width, int height) { }
    public virtual bool OnClose() => true;
    public virtual void OnKeyDown(Key key) { }
    public virtual void OnKeyUp(Key key) { }
    public virtual void OnMouseDown(MouseButton button) { }
    public virtual void OnMouseUp(MouseButton button) { }
    public virtual void OnMouseMove(Vector2 position) { }

    public void Load() {
        Renderer.Load();
        Renderer.Resize((int)Handler.Size.X, (int)Handler.Size.Y);

        Input = new(this);
        Animations = new(this);

        UpdateTime = new(this);
        RenderTime = new(this);

        OnLoad();

        UpdateTime.Load();
        RenderTime.Load();
    }

    public void Update() {
        UpdateTime.Update();
        Animations.Update();
        Element.UpdateSelf();
        OnUpdate();
        Input.Update();
    }

    public void Render() {
        RenderTime.Update();
        if (ClearRenderer) Canvas.Clear(SKColors.Black);
        Element.RenderSelf(Canvas);
        OnRender();
        if (FlushRenderer) Renderer.Flush();
    }

    public void Resize(int width, int height) {
        Renderer.Resize(width, height);
        Element.ResizeSelf(width, height);
        OnResize(width, height);
    }

    public void KeyDown(Key key) {
        Input.HandleKeyDown(key);
        OnKeyDown(key);
    }

    public void KeyUp(Key key) {
        Input.HandleKeyUp(key);
        OnKeyUp(key);
    }

    public void MouseDown(MouseButton button) {
        Input.HandleMouseDown(button);
        OnMouseDown(button);
    }

    public void MouseUp(MouseButton button) {
        Input.HandleMouseUp(button);
        OnMouseUp(button);
    }

    public void MouseMove(Vector2 position) {
        Input.UpdateMousePosition(position);
        OnMouseMove(position);
    }

    public void Close() {
        if (OnClose()) Handler.Close();
    }
}
