#nullable disable

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using System.Diagnostics;
using Yume.Graphics.Elements;

namespace Yume.Graphics.Windowing;

public partial class Window() : GameWindow(GameWindowSettings.Default, new() {
    Flags = ContextFlags.ForwardCompatible,
    Title = "Graphics Window"
}) {
    public GraphicsContext Graphics;

    public new TimeContext RenderTime;
    public new TimeContext UpdateTime;

    /// <summary>
    /// SkiaSharp canvas used for drawing within the render thread
    /// </summary>
    public SKCanvas Canvas => Graphics.Surface.Canvas;

    /// <summary>
    /// The root element that holds all active elements
    /// </summary>
    protected RootElement Element {
        get {
            if (_element == null)
                _element = new(this);
            return _element;
        }
    }
    private RootElement _element;

    /// <summary>
    /// If singlethreaded, this is the primary update/render frequency
    /// <br/>
    /// If multithreaded, this targets only the update thread
    /// </summary>
    public new float UpdateFrequency {
        get => _updateFrequency;
        set {
            _updateFrequency = Math.Max(value, 1);
            base.UpdateFrequency = _updateFrequency;
        }
    }
    private float _updateFrequency = 60;

    /// <summary>
    /// Targets the render thread frequency, only if multithreaded
    /// </summary>
    public new float RenderFrequency {
        get => _renderFrequency;
        set => _renderFrequency = Math.Max(value, 1);
    }
    private float _renderFrequency = 60;

    private GLFWGraphicsContext _threadedContext;
    private readonly object _renderLock = new();

    private Stopwatch _updateTimer;
    private Stopwatch _renderTimer;

    private Thread _renderThread;
    private Thread _updateThread;

    private void MakeGraphicsInstance() {
        lock (_renderLock) {
            Graphics?.Dispose();
            Graphics = new(this);
            Graphics.Load();
        }
    }

    private new void RenderFrame(IGLFWGraphicsContext context = null) {
        lock (_renderLock) {
            Canvas.Clear();
            Element.RenderSelf(Canvas);
            Render();
            Canvas.Flush();

            if (context != null)
                context.SwapBuffers();
        }
    }

    private void RenderThread() {
        _threadedContext.MakeCurrent();
        _renderTimer.Restart();
        MakeGraphicsInstance();

        while (IsMultiThreaded) { 
            RenderTime.Update((float)_renderTimer.Elapsed.TotalSeconds);
            _renderTimer.Restart();

            RenderFrame(_threadedContext);
            Thread.Sleep((int)(1000 / RenderFrequency));
        }
    }

    private void UpdateThread() {
        _updateTimer.Restart();

        while (IsMultiThreaded) {
            UpdateTime.Update((float)_updateTimer.Elapsed.TotalSeconds);
            _updateTimer.Restart();

            Element.UpdateSelf();
            Update();
            
            Thread.Sleep((int)(1000 / UpdateFrequency));
        }
    }

    private void SpawnThreads() {
        _renderThread = new(RenderThread);
        _updateThread = new(UpdateThread);

        _renderThread.Start();
        _updateThread.Start();
    }

    private void JoinThreads() {
        _renderThread?.Join();
        _updateThread?.Join();
    }

    private void HandleMultithreaded(bool threaded) {
        if (threaded) {
            this.Context.MakeNoneCurrent();
            SpawnThreads();
        } else {
            JoinThreads();
            _threadedContext?.MakeNoneCurrent();
            this.Context.MakeCurrent();
            MakeGraphicsInstance();
        }
    }

    public new bool IsMultiThreaded {
        get => _isMultithreaded;
        set {
            if (value == _isMultithreaded)
                return;

            _isMultithreaded = value;
            HandleMultithreaded(value);
        }
    }
    private bool _isMultithreaded = true;

    public virtual void Render() { }
    public virtual void Update() { }
    public new virtual void Load() { }
    public new virtual void Unload() { }
    public new virtual void Resize(Vector2i size) { }
    public new virtual void KeyDown(KeyboardKeyEventArgs e) { }
    public new virtual void KeyUp(KeyboardKeyEventArgs e) { }

    #region Sealed overrides
    protected unsafe sealed override void OnLoad() {
        base.OnLoad();
        base.UpdateFrequency = _updateFrequency;

        RenderTime = new(this);
        UpdateTime = new(this);

        _updateTimer = new();
        _renderTimer = new();

        _threadedContext = new GLFWGraphicsContext(this.WindowPtr);

        Load();
        HandleMultithreaded(IsMultiThreaded);
    }
    
    protected sealed override void OnUnload() {
        base.OnUnload();
        Unload();
    }

    public override void Close() {
        JoinThreads();
        Graphics?.Dispose();
        base.Close();
    }

    public new Vector2i Size => FramebufferSize;
    public Vector2i WindowSize => base.Size;

    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);
        
        // Do not remove the lock, otherwise the graphics context will be disposed
        // on the render thread as it re-instantiates the graphics context
        lock (_renderLock)
            Graphics.Resize(e.Size);
        
        Element.ResizeSelf(e.Size);        
        Resize(e.Size);

        if (IsMultiThreaded) {
            lock (_renderLock) {
                _threadedContext.MakeNoneCurrent();
                Context.MakeCurrent();
                
                RenderFrame(Context);
                
                Context.MakeNoneCurrent();
                _threadedContext.MakeCurrent();
            }
        }
    }

    protected sealed override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        KeyDown(e);
    }

    protected sealed override void OnResize(ResizeEventArgs e) => base.OnResize(e);

    protected sealed override void OnRenderFrame(FrameEventArgs args) {
        if (IsMultiThreaded) return;
        
        base.OnRenderFrame(args);
        RenderTime.Update((float)args.Time);
        
        RenderFrame(Context);
    }

    protected sealed override void OnUpdateFrame(FrameEventArgs args) {
        if (IsMultiThreaded) return;

        base.OnUpdateFrame(args);
        UpdateTime.Update((float)args.Time);
        Element.UpdateSelf();
        Update();
    }
    #endregion
}
