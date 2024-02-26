#nullable disable

using OpenTK.Mathematics;
using SkiaSharp;

public class CircularElementReferenceException : Exception {
    public CircularElementReferenceException() : base("Circular reference detected. Setting this element as its own ancestor or descendant is not allowed.") { }
}

public class Element {
    public Transform Transform {
        get {
            if (_transform == null) {
                _transform = new();
                _transform.Element = this;
            }

            return _transform;
        }
        set {
            if (_transform?.Element == this)
                _transform.Element = null;
            _transform = value ?? new();
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

    public Window Window {
        get => _window;
        protected set {
            _window = value;
            foreach (Element child in Children)
                if (child.Window != value)
                    child.Window = value;
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
            if (Parent != null)
                Parent._children.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
        }
    }
    private int _zIndex;

    public IReadOnlyList<Element> Children => _children.AsReadOnly();
    private List<Element> _children = new();

    public void AddChild(Element child) {
        if (!_children.Contains(child)) {
            int index = 0;
            for (int i = 0; i < _children.Count; i++) {
                if (_children[i].ZIndex > child.ZIndex) {
                    index = i;
                    break;
                }
            }
            _children.Insert(index, child);
        }

        if (child.Parent != this)
            child.Parent = this;
        
        if (!child._isLoaded) {
            child.Load();
            child._isLoaded = true;
        }
    }

    public void RemoveChild(Element child) {
        if (_children.Contains(child))
            _children.Remove(child);
        if (child.Parent == this)
            child.Parent = null;
    }

    public virtual void Render() { }
    public virtual void Update() { }
    public virtual void Resize(Vector2 size) { }
    public virtual void Load() { }
    private bool _isLoaded;

    public void RenderSelf() {
        int count = Window.Canvas.Save();
        Transform.ApplyToCanvas(Window.Canvas);

        if (ShouldRender())
            Render();
        
        foreach (Element child in Children)
            child.RenderSelf();
        Window.Canvas.RestoreToCount(count);
    }

    public void UpdateSelf() {
        Update();
        foreach (Element child in Children)
            child.UpdateSelf();
    }

    public bool ShouldRender() {
        SKPoint ltPos = Window.Canvas.TotalMatrix.MapPoint(new SKPoint(Transform.Size.X, Transform.Size.Y));

        if (ltPos.X < 0 || ltPos.Y < 0)
            return false;

        if (Transform.WorldPosition.X > Window.Size.X || Transform.WorldPosition.Y > Window.Size.Y)
            return false;

        return true;
    }

    public void ResizeSelf(Vector2 size) {
        Resize(size);
        
        foreach (Element child in Children)
            child.ResizeSelf(size);
    }
}

public class BoxElement : Element {
    public SKColor Color {
        get => Paint.Color;
        set {
            Paint.Color = value;
            Paint.Style = SKPaintStyle.Fill;
        }
    }
    public SKPaint Paint { get; private set; } = new();

    public override void Render() =>
        Window.Canvas.DrawRect(0, 0, Transform.Size.X, Transform.Size.Y, Paint);
}

public class SimpleText : Element {
    public string Text = "";

    public int FontSize {
        get => _fontSize;
        set {
            _fontSize = value;
            Paint.TextSize = value;
        }
    }
    private int _fontSize = 24;

    public SKColor TextColor {
        get => Paint.Color;
        set {
            Paint.Color = value;
            Paint.Style = SKPaintStyle.Fill;
        }
    }
    public SKPaint Paint { get; private set; } = new() {
        IsAntialias = true,
        Color = SKColors.White,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Left,
        TextSize = 24,
        Typeface = SKTypeface.FromFamilyName("Helvetica")
    };

    public override void Render() {
        Window.Canvas.DrawText(Text, 0, FontSize, Paint);
    }
}