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
    private bool _isMultithreaded = true;
    private float _renderFrequency = 60;

    private Thread _renderThread;
    private Stopwatch _renderTimer;

    private GLFWGraphicsContext _threadedContext;
    private float _updateFrequency = 60;

    private Thread _updateThread;
    private Stopwatch _updateTimer;
    public AnimationContext Animations;

    public GraphicsContext Graphics;
    public InputContext Input;

    public new TimeContext RenderTime;
    public new TimeContext UpdateTime;

    /// <summary>
    ///     SkiaSharp canvas used for drawing within the render thread
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

    public new bool IsMultiThreaded {
        get => _isMultithreaded;
        set {
            if (value == _isMultithreaded)
                return;

            _isMultithreaded = value;
            HandleMultithreaded(value);
        }
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

        _threadedContext!.MakeNoneCurrent();
    }

    private void UpdateThread() {
        _updateTimer.Restart();

        while (IsMultiThreaded) {
            UpdateTime.Update((float)_updateTimer.Elapsed.TotalSeconds);
            _updateTimer.Restart();

            Animations.Update();
            Element.UpdateSelf();
            Update();

            Input.Update();
            Thread.Sleep((int)(1000 / UpdateFrequency));
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
        if (threaded) {
            Context.MakeNoneCurrent();
            SpawnThreads();
        } else {
            JoinThreads();
            Console.WriteLine(Context.IsCurrent + ", " + _threadedContext.IsCurrent);
            Context.MakeCurrent();
            Console.WriteLine(Context.IsCurrent + ", " + _threadedContext.IsCurrent);
            MakeGraphicsInstance();
        }
    }

    protected virtual void Render() { }
    protected virtual void Update() { }
    protected new virtual void Load() { }
    protected new virtual void Unload() { }
    protected new virtual void Resize(Vector2i size) { }

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
        lock (_renderLock) {
            Graphics.Resize(e.Size);
        }

        Element.ResizeSelf(e.Size);
        Resize(e.Size);
    }

    protected sealed override void OnKeyDown(KeyboardKeyEventArgs e) {
        base.OnKeyDown(e);
        Input.KeyDown(e.Key);
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        Input.KeyUp(e.Key);
        base.OnKeyUp(e);
    }

    protected sealed override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
    }

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

        Animations.Update();
        Element.UpdateSelf();
        Update();
        Input.Update();
    }

    #endregion
}