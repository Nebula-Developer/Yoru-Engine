#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Platforms;

namespace Yoru;

public class Application {
    public readonly object RenderLock = new();
    private Vector2 _canvasScale = Vector2.One;
    private RootElement _element;
    public IApplicationHandler Handler { get; set; } = new HeadlessHandler();
    
    public bool FlushRenderer { get; set; } = true;
    public bool ClearCanvas { get; set; } = true;
    public Renderer Renderer { get; set; } = new EmptyRenderer();
    public ElementDrawingMethod DrawingMethod { get; set; } = ElementDrawingMethod.Render;
    
    public Vector2 Size { get; private set; }
    public Vector2 FramebufferSize { get; private set; }
    public SKCanvas AppCanvas {
        get => Renderer.Surface.Canvas;
    }
    public Vector2 CanvasScale {
        get => _canvasScale;
        set {
            value = new(MathF.Max(value.X, 1), MathF.Max(value.Y, 1));
            _canvasScale = value;
            if (Renderer.Surface != null)
                Resize((int)FramebufferSize.X, (int)FramebufferSize.Y, value);
        }
    }
    
    public TimeContext UpdateTime { get; protected set; }
    public TimeContext RenderTime { get; protected set; }
    
    public InputContext Input { get; protected set; }
    
    public AnimationContext Animations { get; protected set; }
    public DebugContext Debugging { get; protected set; }
    
    public RootElement Element {
        get {
            if (_element == null) _element = new(this);
            return _element;
        }
    }
    
    public void Load() {
        Renderer.Load();
        
        Animations = new(this);
        Input = new(this);
        
        Input.Load();
        Animations.Load();
        Debugging = new(this);
        
        UpdateTime = new(this);
        RenderTime = new(this);
        
        OnLoad();
        Resize((int)Handler.Size.X, (int)Handler.Size.Y);
        
        UpdateTime.Load();
        RenderTime.Load();
    }
    
    public void Update() {
        Input.Update();
        UpdateTime.Update();
        Animations.Update();
        
        Element.Update();
        OnUpdate();
        Input.UpdateCollections();
    }
    
    public void Render() {
        lock (RenderLock) {
            Debugging.Update();
            RenderTime.Update();
            
            AppCanvas.ResetMatrix();
            AppCanvas.Scale(CanvasScale.X, CanvasScale.Y);
            if (ClearCanvas) AppCanvas.Clear(SKColors.Black);
            
            Element.Render(AppCanvas);
            OnRender();
            
            if (FlushRenderer) Renderer.Flush();
        }
    }
    
    public void ResizeRoot() => Element.Resize((int)Size.X, (int)Size.Y);
    
    public void Resize(int width, int height, Vector2 canvasScale) { // Resizing as the actual window frame size, not handling the DPI
        _canvasScale = canvasScale;
        
        FramebufferSize = new(width, height);
        Size = new(width / CanvasScale.X, height / CanvasScale.Y);
        ResizeRoot();
        
        lock (RenderLock) Renderer.Resize(width, height);
        OnResize(width, height);
    }
    
    public void Resize(int width, int height) => Resize(width, height, CanvasScale);
    
    public void ResizeElement(int width, int height, Vector2 canvasScale) => Resize((int)(width * canvasScale.X), (int)(height * canvasScale.Y), canvasScale);
    public void ResizeElement(int width, int height) => Resize((int)(width * CanvasScale.X), (int)(height * CanvasScale.Y));
    
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
    
    protected virtual void OnLoad() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnRender() { }
    protected virtual void OnResize(int width, int height) { }
    protected virtual bool OnClose() => true;
    protected virtual void OnKeyDown(Key key) { }
    protected virtual void OnKeyUp(Key key) { }
    protected virtual void OnMouseDown(MouseButton button) { }
    protected virtual void OnMouseUp(MouseButton button) { }
    protected virtual void OnMouseMove(Vector2 position) { }
}
