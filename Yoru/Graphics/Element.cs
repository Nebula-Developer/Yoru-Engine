#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Input;

namespace Yoru.Graphics;

public class Element {
    private readonly List<Element> _children = new();
    
    private Application _app;
    private bool _isLoaded;
    
    private Element _parent;
    
    private Transform _transform;
    
    private int _zIndex;
    public bool Cull { get; set; } = true;
    public bool MouseInteractions { get; set; } = false;
    
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
    
    protected Application App {
        get => _app;
        set {
            _app = value;
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

    protected virtual void Update() { }
    protected virtual void Resize(int width, int height) { }
    protected virtual void Load() { }

    public virtual bool MouseDown(MouseButton button) => true;
    public virtual bool MouseUp(MouseButton button) => true;
    public virtual void MouseMove(Vector2 position) { }
    public virtual void MouseEnter() { }
    public virtual void MouseLeave() { }
    public virtual void MouseDrag() { }
    
    protected virtual void Render(SKCanvas canvas) { }
    protected virtual void ChildAdded(Element child) { }
    protected virtual void ChildRemoved(Element child) { }
    protected virtual void RenderChildren(SKCanvas canvas) => ForChildren(child => child.RenderSelf(canvas));
    
    protected virtual void TransformChanged() { }
    
    public virtual bool CheckMouseIntersect(Vector2 position) {
        if (Transform.Size.X == 0 || Transform.Size.Y == 0) return false;
        
        if (position.X >= Transform.WorldPosition.X && position.X <= Transform.WorldPosition.X + Transform.Size.X &&
            position.Y >= Transform.WorldPosition.Y && position.Y <= Transform.WorldPosition.Y + Transform.Size.Y) {
            return true;
        }
        
        return false;
    }
    
    protected virtual bool ShouldRender(SKCanvas canvas)
        => !canvas.QuickReject(new SKRect(0, 0, Transform.Size.X, Transform.Size.Y));
    
    public void RenderSelf(SKCanvas canvas) {
        if (App == null) return;
        
        var count = canvas.Save();
        Transform.ApplyToCanvas(canvas);
        
        if (!Cull || ShouldRender(canvas))
            Render(canvas);
        
        RenderChildren(canvas);
        canvas.RestoreToCount(count);
    }
    
    public void UpdateSelf() {
        if (MouseInteractions) {
            if (CheckMouseIntersect(App.Input.MousePosition))
                App.Input.HandleElementEnter(this);
            else
                App.Input.HandleElementLeave(this);
        }

        Update();
        ForChildren(child => child.UpdateSelf());
    }
    
    public void ResizeSelf(int width, int height) {
        Resize(width, height);
        ForChildren(child => child.ResizeSelf(width, height));
    }
}
