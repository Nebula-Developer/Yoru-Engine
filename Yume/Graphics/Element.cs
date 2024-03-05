#nullable disable

using OpenTK.Mathematics;
using SkiaSharp;
using Yume.Windowing;
using Yume.Graphics;

namespace Yume.Graphics.Elements;

public class Element {
    private readonly List<Element> _children = new();
    private bool _isLoaded;

    private Element _parent;

    private Transform _transform;

    private Window _window;

    private int _zIndex;

    public Transform Transform {
        get {
            if (_transform != null) return _transform;

            // Using object initializer would cause a stack overflow,
            // as it would infinitely recurse into this getter

            // ReSharper disable once UseObjectOrCollectionInitializer
            _transform = new Transform();
            _transform.Element = this;

            return _transform;
        }
        set {
            if (_transform?.Element == this)
                _transform.Element = null;
            _transform = value ?? new Transform();
            if (_transform.Element != this)
                _transform.Element = this;
        }
    }

    protected Window Window {
        get => _window;
        set {
            _window = value;
            ForChildren(child => {
                if (child.Window != value)
                    child.Window = value;
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

            Window = Parent?.Window;
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

    public IReadOnlyList<Element> Children => _children.AsReadOnly();

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

        if (child._isLoaded) return;
        child.Load();
        child._isLoaded = true;

        ChildAdded(child);
    }

    public void RemoveChild(Element child) {
        _children.Remove(child);
        ChildRemoved(child);
        if (child.Parent == this)
            child.Parent = null;
    }

    protected virtual void Update() { }
    protected virtual void Resize(Vector2 size) { }
    protected virtual void Load() { }
    protected virtual void Render(SKCanvas canvas) { }
    protected virtual void ChildAdded(Element child) { }
    protected virtual void ChildRemoved(Element child) { }

    protected virtual void RenderChildren(SKCanvas canvas) {
        ForChildren(child => child.RenderSelf(canvas));
    }
    
    protected virtual bool ShouldRender() =>
        !Window.Canvas.QuickReject(new SKRect(0, 0, Transform.Size.X, Transform.Size.Y));

    public void RenderSelf(SKCanvas canvas) {
        if (Window == null) return;

        var count = Window.Canvas.Save();
        Transform.ApplyToCanvas(Window.Canvas);

        if (ShouldRender())
            Render(canvas);

        RenderChildren(canvas);
        Window.Canvas.RestoreToCount(count);
    }

    public void UpdateSelf() {
        Update();
        ForChildren(child => child.UpdateSelf());
    }
    
    public void ResizeSelf(Vector2 size) {
        Resize(size);
        ForChildren(child => child.ResizeSelf(size));
    }
}