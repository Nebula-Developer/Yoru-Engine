#nullable disable

using OpenTK.Mathematics;
using SkiaSharp;
using Yume.Graphics.Windowing;

namespace Yume.Graphics.Elements;

public class CircularElementReferenceException()
    : Exception("Circular reference detected. Setting this element as its own ancestor or descendant is not allowed.");

public class Element {
    public Transform Transform {
        get {
            if (_transform != null) return _transform;
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

    private Transform _transform;

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

    protected Window Window {
        get => _window;
        set {
            _window = value;
            ForChildren((child) => {
                if (child.Window != value)
                    child.Window = value;
            });
        }
    }

    private Window _window;

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

    private Element _parent;

    public int ZIndex {
        get => _zIndex;
        set {
            _zIndex = value;
            Parent?._children.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }
    }

    private int _zIndex;

    public IReadOnlyList<Element> Children => _children.AsReadOnly();
    private readonly List<Element> _children = new();

    public void ForChildren(Action<Element> action) {
        // Don't use foreach, as the list can be modified during iteration
        for (int i = 0; i < _children.Count; i++)
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
    }

    public void RemoveChild(Element child) {
        _children.Remove(child);
        if (child.Parent == this)
            child.Parent = null;
    }

    public virtual void Update() { }
    public virtual void Resize(Vector2 size) { }
    public virtual void Load() { }
    private bool _isLoaded;
    public virtual void Render(SKCanvas canvas) { }

    public virtual void RenderChildren(SKCanvas canvas) {
        ForChildren((child) => child.RenderSelf(canvas));
    }

    public void RenderSelf(SKCanvas canvas) {
        if (Window == null) return;
        
        int count = Window.Canvas.Save();
        Transform.ApplyToCanvas(Window.Canvas);

        if (ShouldRender())
            Render(canvas);

        RenderChildren(canvas);
        Window.Canvas.RestoreToCount(count);
    }

    public void UpdateSelf() {
        Update();
        ForChildren((child) => child.UpdateSelf());
    }

    protected virtual bool ShouldRender() {
        SKRect rect = new(0, 0, Transform.Size.X, Transform.Size.Y);
        return !Window.Canvas.QuickReject(rect);
    }

    public void ResizeSelf(Vector2 size) {
        Resize(size);
        ForChildren((child) => child.ResizeSelf(size));
    }
}
