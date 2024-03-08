#nullable disable


using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using Yume.Graphics;
using Yume.Graphics.Elements;
using Yume.Input;


namespace Yume.Windowing;

public class Window() : GameWindow(GameWindowSettings.Default, new NativeWindowSettings {
    Flags = ContextFlags.ForwardCompatible,
    Title = "Yume Window"
}) {
    private readonly object _renderLock = new();
    private RootElement _element;
    private GLFWGraphicsContext _threadedContext;

    private Thread _renderThread;
    private Stopwatch _renderTimer;

    public new bool IsMultiThreaded = true;
    private float _renderFrequency = 60;
    private float _updateFrequency = 60;

    private Thread _updateThread;
    private Stopwatch _updateTimer;

    public AnimationContext Animations;
    public GraphicsContext Graphics;
    public InputContext Input;

    public new TimeContext RenderTime;
    public new TimeContext UpdateTime;

    public new Vector2i Size => FramebufferSize;
    public Vector2i WindowSize => base.Size;

    /// <summary>
    ///     SkiaSharp canvas used for drawing directly
    ///     to the window within the render thread
    /// </summary>
    public SKCanvas Canvas => Graphics.Surface.Canvas;

    /// <summary>
    ///     The root element that holds all active elements
    /// </summary>
    protected RootElement Element {
        get {
            if (_element == null)
                _element = new RootElement(this);
            return _element;
        }
    }

    /// <summary>
    /// Controls both the <see cref="UpdateFrequency" />
    /// and <see cref="RenderFrequency" />
    /// </summary>
    public float Frequency {
        set {
            UpdateFrequency = value;
            RenderFrequency = value;
        }
    }

    /// <summary>
    ///     If singlethreaded, this is the primary update/render frequency
    ///     <br />
    ///     If multithreaded, this targets only the update thread
    /// </summary>
    public new float UpdateFrequency {
        get => _updateFrequency;
        set {
            _updateFrequency = Math.Max(value, 1);
            base.UpdateFrequency = _updateFrequency;
        }
    }

    /// <summary>
    ///     Targets the render thread frequency, only if multithreaded
    /// </summary>
    public new float RenderFrequency {
        get => _renderFrequency;
        set => _renderFrequency = Math.Max(value, 1);
    }

    private void MakeGraphicsInstance() {
        lock (_renderLock) {
            Graphics?.Dispose();
            Graphics = new GraphicsContext(this);
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

    private new void UpdateFrame() {
        Animations.Update();
        Element.UpdateSelf();
        Update();
        Input.Update();
    }

    private int _threadCountLock = 0;

    private void RenderThread() {
        int threadCountLock = _threadCountLock;
        _renderTimer.Restart();

        _threadedContext.MakeCurrent();
        MakeGraphicsInstance();


        while (IsMultiThreaded && threadCountLock == _threadCountLock) {
            RenderFrame(_threadedContext);

            Thread.Sleep((int)(1000 / RenderFrequency));
            RenderTime.Update((float)_renderTimer.Elapsed.TotalSeconds);
            _renderTimer.Restart();
        }

        _threadedContext.MakeNoneCurrent();
    }

    private void UpdateThread() {
        int threadCountLock = _threadCountLock;
        _updateTimer.Restart();


        while (IsMultiThreaded && threadCountLock == _threadCountLock) {
            UpdateFrame();

            Thread.Sleep((int)(1000 / UpdateFrequency));
            UpdateTime.Update((float)_updateTimer.Elapsed.TotalSeconds);
            _updateTimer.Restart();
        }
    }

    private void SpawnThreads() {
        _renderThread = new Thread(RenderThread);
        _updateThread = new Thread(UpdateThread);

        _renderThread.Start();
        _updateThread.Start();
    }

    private void JoinThreads() {
        _renderThread?.Join();
        _updateThread?.Join();
    }

    private void HandleMultithreaded(bool threaded) {
        Input.Update();
        _threadCountLock++;
        JoinThreads();

        // TODO: Ensure this is only executed within the main thread, as
        // Context.MakeNoneCurrent() can only be from same thread as Context.MakeCurrent()
        lock (_renderLock) {
            if (threaded) {
                Context.MakeNoneCurrent();
                SpawnThreads();
            } else {
                Context.MakeCurrent();
                MakeGraphicsInstance();
            }
        }
    }

    #region Sealed overrides

    protected sealed override unsafe void OnLoad() {
        base.OnLoad();
        base.UpdateFrequency = _updateFrequency;

        Animations = new AnimationContext(this);
        Input = new InputContext(this);

        RenderTime = new TimeContext(this);
        UpdateTime = new TimeContext(this);

        _updateTimer = new Stopwatch();
        _renderTimer = new Stopwatch();

        _threadedContext = new GLFWGraphicsContext(WindowPtr);

        Load();
        HandleMultithreaded(IsMultiThreaded);
    }

    protected sealed override void OnUnload() {
        base.OnUnload();
        Unload();
    }

    public sealed override void Close() {
        JoinThreads();
        Graphics?.Dispose();
        base.Close();
    }

    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        // Do not remove the lock, otherwise the graphics context will be disposed
        // on the render thread as it re-instantiates the graphics context
        lock (_renderLock) {
            Graphics.Resize(e.Size);
        }

        Element.ResizeSelf(e.Size);
        Resize(e.Size);
    }

    protected sealed override void OnKeyDown(KeyboardKeyEventArgs e) => Input.KeyDown(e.Key);
    protected sealed override void OnKeyUp(KeyboardKeyEventArgs e) => Input.KeyUp(e.Key);

    protected sealed override void OnResize(ResizeEventArgs e) => base.OnResize(e);

    protected sealed override void OnRenderFrame(FrameEventArgs args) {
        if (IsMultiThreaded) {
            if (Context.IsCurrent || !_renderThread.IsAlive)
                HandleMultithreaded(true);
            return;
        }

        // Only executed on main thread, therefore
        // ensure that this is the only location we
        // call HandleMultithreaded(false), unless
        // it is safely on the main thread.

        // TODO: Implement better null singlethread checking
        if (!Context.IsCurrent || Graphics.Surface == null)
            HandleMultithreaded(false);

        base.OnRenderFrame(args);
        RenderTime.Update(args.Time);

        RenderFrame(Context);
    }

    protected sealed override void OnUpdateFrame(FrameEventArgs args) {
        if (IsMultiThreaded) return;

        base.OnUpdateFrame(args);
        UpdateTime.Update(args.Time);

        UpdateFrame();
    }

    #endregion

    #region Virtuals

    protected virtual void Render() { }
    protected virtual void Update() { }
    protected new virtual void Load() { }
    protected new virtual void Unload() { }
    protected new virtual void Resize(Vector2i size) { }

    #endregion
}