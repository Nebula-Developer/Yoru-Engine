#nullable disable

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using System.Diagnostics;

public class RootElement : Element {
    public RootElement(Window window) {
        this.Window = window;
        Transform.Size = window.Size;
    }


    public override void Resize(Vector2 size) {
        Console.WriteLine("Resized root elm to: " + size);
        Transform.Size = size;
    }
}

public partial class Window() : GameWindow(GameWindowSettings.Default, new() {
    Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
    Title = "Graphics Window",
    // TransparentFramebuffer = true,
    // WindowBorder = WindowBorder.Hidden
}) {
    /// <summary>
    /// Graphical context for the window (OpenTK / SkiaSharp wrapper)
    /// </summary>
    public GraphicsContext Graphics;

    /// <summary>
    /// TimeContext for the Render thread
    /// </summary>
    public new TimeContext RenderTime;

    /// <summary>
    /// TimeContext for the Update thread
    /// </summary>
    public new TimeContext UpdateTime;

    /// <summary>
    /// SkiaSharp canvas used for drawing within the render thread
    /// </summary>
    public SKCanvas Canvas => Graphics.Surface.Canvas;

    /// <summary>
    /// The root element that holds all active elements
    /// </summary>
    public RootElement Element {
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
    private float _updateFrequency = 144;

    /// <summary>
    /// Targets the render thread frequency, only if multithreaded
    /// </summary>
    public new float RenderFrequency {
        get => _renderFrequency;
        set => _renderFrequency = Math.Max(value, 1);
    }
    private float _renderFrequency = 144;

    internal GLFWGraphicsContext _threadedContext;
    private readonly object _renderLock = new();

    internal Stopwatch _updateTimer;
    internal Stopwatch _renderTimer;

    internal Thread _renderThread;
    internal Thread _updateThread;

    internal void MakeGraphicsInstance() {
        lock (_renderLock) {
            Graphics?.Dispose();
            Graphics = new(this);
            Graphics.Load();
        }
    }

    internal new void RenderFrame(IGLFWGraphicsContext context = null) {
        lock (_renderLock) {
            Canvas.Clear();
            Element.RenderSelf();
            Render();
            Canvas.Flush();
        }
        
        if (context != null)
            context.SwapBuffers();
    }


    /// <summary>
    /// Multithreaded render loop method
    /// </summary>
    internal unsafe void RenderThread() {
        unsafe {
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
    }

    /// <summary>
    /// Multithreaded update loop method
    /// </summary>
    internal unsafe void UpdateThread() {
        _updateTimer.Restart();

        while (IsMultiThreaded) {
            UpdateTime.Update((float)_updateTimer.Elapsed.TotalSeconds);
            _updateTimer.Restart();

            Element.UpdateSelf();
            Update();
            
            Thread.Sleep((int)(1000 / UpdateFrequency));
        }
    }

    /// <summary>
    /// Instantiates and starts the render and update threads
    /// </summary>
    internal void SpawnThreads() {
        _renderThread = new(RenderThread);
        _updateThread = new(UpdateThread);

        _renderThread.Start();
        _updateThread.Start();
    }

    internal void JoinThreads() {
        _renderThread?.Join();
        _updateThread?.Join();
    }

    internal void HandleMultithreaded(bool threaded) {
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

    /// <summary>
    /// If true, the window will use multithreading for rendering and updating
    /// </summary>
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

    /// <summary>
    /// Invokable render method for inheriting classes
    /// </summary>
    public virtual void Render() { }

    /// <summary>
    /// Invokable update method for inheriting classes
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Invokable load method for inheriting classes
    /// </summary>
    public new virtual void Load() { }

    /// <summary>
    /// Invokable resize method for inheriting classes
    /// </summary>
    /// <param name="size"></param>
    public new virtual void Resize(Vector2i size) { }

    /// <summary>
    /// Invokable keydown method for inheriting classes
    /// </summary>
    /// <param name="e"></param>
    public new virtual void KeyDown(KeyboardKeyEventArgs e) { }

    /// <summary>
    /// Invokable keyup method for inheriting classes
    /// </summary>
    /// <param name="e"></param>
    public new virtual void KeyUp(KeyboardKeyEventArgs e) { }

    #region Sealed overrides
    protected unsafe sealed override void OnLoad() {
        base.OnLoad();

        RenderTime = new(this);
        UpdateTime = new(this);

        _updateTimer = new();
        _renderTimer = new();

        _threadedContext = new GLFWGraphicsContext(this.WindowPtr);

        // The load method comes before our multithread handling
        // to ensure we load before instantiating our threads, or
        // enabling the singlethreaded listeners (which is handled
        // by opentk)
        Load();
        HandleMultithreaded(IsMultiThreaded);
    }

    public new Vector2i Size => FramebufferSize;
    public Vector2i WindowSize => base.Size;

    public bool UseResizeRefreshing = true;

    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        // Do not remove the lock, otherwise the graphics context will be disposed
        // on the render thread as it reinstantiates the graphics context
        lock (_renderLock)
            Graphics.Resize(e.Size);

        Element.ResizeSelf(e.Size);        
        Resize(e.Size);

        if (UseResizeRefreshing && IsMultiThreaded) {
            RenderTime.Update((float)_renderTimer.Elapsed.TotalSeconds);
            _renderTimer.Restart();
            
            _threadedContext.MakeNoneCurrent();
            this.Context.MakeCurrent();

            // FrameBufferResize is called from the main thread, therefore
            // we need to use RenderFrame via the main thread's context
            RenderFrame(this.Context);

            _threadedContext.MakeCurrent();
        }
    }

    protected sealed override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        KeyDown(e);
    }

    protected sealed override void OnResize(ResizeEventArgs e) => base.OnResize(e);

    /// <summary>
    /// Singlethreaded render method
    /// </summary>
    /// <param name="args"></param>
    protected sealed override void OnRenderFrame(FrameEventArgs args) {
        if (IsMultiThreaded) return;
        
        base.OnRenderFrame(args);
        RenderTime.Update((float)args.Time);
        
        RenderFrame(Context);
    }

    /// <summary>
    /// Singlethreaded update method
    /// </summary>
    /// <param name="args"></param>
    protected sealed override void OnUpdateFrame(FrameEventArgs args) {
        if (IsMultiThreaded) return;

        base.OnUpdateFrame(args);
        UpdateTime.Update((float)args.Time);
        Element.UpdateSelf();
        Update();
    }
    #endregion
}
