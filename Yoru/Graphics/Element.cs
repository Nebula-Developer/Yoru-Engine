#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Input;

namespace Yoru.Graphics;

public class Element : IDisposable {
    private readonly List<Element> _children = new();
    
    private Application _app;
    private bool _isLoaded;
    
    private Element _parent;
    
    private Transform _transform;
    
    private int _zIndex;
    public bool Cull { get; set; } = true;
    public bool MouseInteraction { get; set; } = true;
    public bool ApplyTransformMatrix { get; set; } = true;
    
    public bool IsMouseOver { get; private set; }
    public Dictionary<MouseButton, bool> IsButtonDown { get; } = new();
    public bool IsMouseDown {
        get => IsButtonDown.Any(pair => pair.Value);
    }
    
    public Transform Transform {
        get {
            if (_transform != null) return _transform;
            
            // Using object initializer would cause a stack overflow,
            // as it would infinitely recurse into this getter
            
            // ReSharper disable once UseObjectOrCollectionInitializer
            _transform = new();
            _transform.Element = this;
            
            return _transform;
        }
        set {
            if (_transform == value) return;
            
            var previousTransform = _transform;
            _transform = value ?? new Transform();
            
            if (previousTransform?.Element == this)
                previousTransform.Element = null;
            
            if (_transform.Element != this)
                _transform.Element = this;
            
            TransformChanged();
        }
    }
    
    public Application App {
        get => _app;
        protected set {
            _app = value;
            Transform.UpdateMatrix();
            ForChildren(child => {
                if (child.App != value)
                    child.App = value;
            });
        }
    }
    
    public Element Parent {
        get => _parent;
        set {
            if (HasCircularReference(value))
                throw new CircularElementReferenceException();
            
            if (_parent != null && _parent.Children.Contains(this))
                _parent.RemoveChild(this);
            _parent = value;
            if (_parent != null && !_parent.Children.Contains(this))
                _parent.AddChild(this);
            
            App = Parent?.App;
            Transform.UpdateTransforms();
        }
    }
    
    public int ZIndex {
        get => _zIndex;
        set {
            _zIndex = value;
            Parent?._children.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }
    }
    
    public IReadOnlyList<Element> Children {
        get => _children.AsReadOnly();
    }
    
    public Action DoUpdate { get; set; }
    public Action<SKCanvas> DoRender { get; set; }
    public Action<int, int> DoResize { get; set; }
    public Action DoLoad { get; set; }
    public Action DoUnload { get; set; }
    
    public Func<MouseButton, bool> DoMouseDown { get; set; }
    public Action<MouseButton> DoMouseUp { get; set; }
    public Action<Vector2> DoMouseMove { get; set; }
    public Func<bool> DoMouseEnter { get; set; }
    public Action DoMouseLeave { get; set; }
    
    public Action DoChildAdded { get; set; }
    public Action DoChildRemoved { get; set; }
    public Action DoTransformChanged { get; set; }
    public Action DoTransformValueChanged { get; set; }
    
    public virtual SKPath Path { get; private set; } = new();
    
    public void Dispose() {
        Unload();
        GC.SuppressFinalize(this);
    }
    
    private bool HasCircularReference(Element potentialParent) {
        if (potentialParent == null)
            return false;
        
        var ancestor = potentialParent;
        while (ancestor != null) {
            if (ancestor == this)
                return true;
            ancestor = ancestor.Parent;
        }
        
        return false;
    }
    
    public void ForChildren(Action<Element> action) {
        // Don't use foreach, as the list can be modified during iteration
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _children.Count; i++)
            action(_children[i]);
    }
    
    public void AddChild(Element child) {
        if (!_children.Contains(child)) {
            _children.Add(child);
            _children.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }
        
        if (child.Parent != this)
            child.Parent = this;
        
        if (!child._isLoaded) {
            child.Load();
            child._isLoaded = true;
        }
        
        ChildAdded(child);
    }
    
    public void RemoveChild(Element child) {
        if (_children.Contains(child)) {
            _children.Remove(child);
            ChildRemoved(child);
        }
        
        if (child.Parent == this)
            child.Parent = null;
    }

    public void ClearChildren() {
        for (var i = 0; i < _children.Count; i++)
            RemoveChild(_children[i]);
    }

    public void AddChildren(params Element[] children) {
        foreach (var child in children)
            AddChild(child);
    }
    
    protected virtual void OnUpdate() { }
    protected virtual void OnRender(SKCanvas canvas) { }
    protected virtual void OnResize(int width, int height) { }
    protected virtual void OnLoad() { }
    protected virtual void OnUnload() { }
    protected virtual void OnRenderChildren(SKCanvas canvas) => ForChildren(child => child.Render(canvas));
    
    protected virtual bool OnMouseDown(MouseButton button) => false;
    protected virtual void OnMouseUp(MouseButton button) { }
    protected virtual void OnMouseMove(Vector2 position) { }
    protected virtual bool OnMouseEnter() => false;
    protected virtual void OnMouseLeave() { }
    
    protected virtual void OnChildAdded(Element child) { }
    protected virtual void OnChildRemoved(Element child) { }
    protected virtual void OnTransformChanged() { }
    protected virtual void OnTransformValueChanged() {
        var points = Transform.Matrix.MapPoints(
            new SKPoint[] {
                new(0, 0),
                new(Transform.Size.X, 0),
                new(Transform.Size.X, Transform.Size.Y),
                new(0, Transform.Size.Y),
                new(0, 0)
            }
        );
        
        Path = new();
        for (var i = 0; i < points.Length; i++) {
            if (i == 0) Path.MoveTo(points[i]);
            else Path.LineTo(points[i]);
        }
    }
    
    public virtual bool PointIntersects(Vector2 position) {
        if (Transform.Size.X == 0 || Transform.Size.Y == 0) return false;
        return Path.Contains(position.X, position.Y);
    }
    
    protected virtual bool ShouldRender(SKCanvas canvas, bool matrixApplied = true)
        => matrixApplied
            ? !canvas.QuickReject(new SKRect(0, 0, Transform.Size.X, Transform.Size.Y))
            : !canvas.QuickReject(new SKRect(0, 0, Transform.Size.X, Transform.Size.Y));
    
    public void Update() {
        OnUpdate();
        DoUpdate?.Invoke();
        ForChildren(child => child.Update());
    }
    
    public void Render(SKCanvas canvas) {
        using SKAutoCanvasRestore restore = new(canvas);
        if (App == null) {
            canvas.SetMatrix(Transform.Matrix);

            if (!Cull || ShouldRender(canvas, false)) {
                OnRender(canvas);
                DoRender?.Invoke(canvas);
                OnRenderChildren(canvas);
            }
                
            return;
        }

        App.Debugging.RenderDepth++;
        
        switch (App.DrawingMethod) {
            case ElementDrawingMethod.Render:
                if (ApplyTransformMatrix) Transform.ApplyToCanvas(canvas);
                if (!Cull || ShouldRender(canvas)) {
                    OnRender(canvas);
                    DoRender?.Invoke(canvas);
                    OnRenderChildren(canvas);
                }
                
                break;
            case ElementDrawingMethod.PathOutline:
                using (SKAutoCanvasRestore _ = new(canvas)) {
                    var color = new SKColor(
                        (byte)(App.Debugging.RenderDepth % 25 * 10),
                        (byte)(App.Debugging.RenderDepth % 10 * 25),
                        255
                    );
                    
                    canvas.DrawPoints(SKPointMode.Polygon, Path.Points, new() {
                        Color = color,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1,
                        FilterQuality = SKFilterQuality.High,
                        IsAntialias = true,
                        IsDither = true,
                        IsStroke = true
                    });
                    
                    OnRenderChildren(canvas);
                }
                
                break;
        }
    }
    
    public void Resize(int width, int height) {
        OnResize(width, height);
        DoResize?.Invoke(width, height);
        ForChildren(child => child.Resize(width, height));
    }
    
    private static bool ReturnVirtualPair(Func<bool> func, Func<bool> action) {
        var resA = func?.Invoke();
        var resB = action?.Invoke();
        return resA ?? resB ?? false;
    }
    
    private static void InvokeVirtualPair(Action method, Action action) {
        method?.Invoke();
        action?.Invoke();
    }
    
    public void Load() => InvokeVirtualPair(OnLoad, DoLoad);
    public bool MouseDown(MouseButton button) => ReturnVirtualPair(() => {
        IsButtonDown[button] = true;
        return OnMouseDown(button);
    }, () => DoMouseDown?.Invoke(button) ?? false);
    public void MouseUp(MouseButton button) => InvokeVirtualPair(() => {
        IsButtonDown[button] = false;
        OnMouseUp(button);
    }, () => DoMouseUp?.Invoke(button));
    public void MouseMove(Vector2 position) => InvokeVirtualPair(() => OnMouseMove(position), () => DoMouseMove?.Invoke(position));
    public bool MouseEnter() => ReturnVirtualPair(() => {
        IsMouseOver = true;
        return OnMouseEnter();
    }, DoMouseEnter);
    public void MouseLeave() => InvokeVirtualPair(() => {
        IsMouseOver = false;
        OnMouseLeave();
    }, DoMouseLeave);
    public void ChildAdded(Element child) => InvokeVirtualPair(() => OnChildAdded(child), DoChildAdded);
    public void ChildRemoved(Element child) => InvokeVirtualPair(() => OnChildRemoved(child), DoChildRemoved);
    public void TransformChanged() => InvokeVirtualPair(OnTransformChanged, DoTransformChanged);
    public void TransformValueChanged() => InvokeVirtualPair(OnTransformValueChanged, DoTransformValueChanged);
    
    public void Unload() => InvokeVirtualPair(OnUnload, DoUnload);
    
    public override bool Equals(object obj) {
        if (obj is Element element)
            return element == this;
        return false;
    }
    
    public override int GetHashCode() => base.GetHashCode();
}
