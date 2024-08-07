#nullable disable

using System.Numerics;
using SkiaSharp;

namespace Yoru.Graphics;

public class Transform {
    private Element _element;
    
    
    public Action DoValueChanged;
    
    public Vector2 RotationOffset = new(0, 0);
    
    public Element Element {
        get => _element;
        set {
            if (_element == value) return;
            
            var previousElement = _element;
            _element = value;
            
            if (previousElement != null && previousElement.Transform == this)
                previousElement.Transform = null;
            
            if (_element != null && _element.Transform != this)
                _element.Transform = this;
            
            UpdateTransforms();
        }
    }
    
    public SKMatrix Matrix { get; private set; }
    
    private void ExecuteChildren(Action<Element> action) {
        Element?.ForChildren(action);
    }
    
    public void UpdateTransforms() {
        UpdateSize();
        UpdatePosition();
        UpdateRotation();
        ValueChanged();
    }
    
    public void ApplyToCanvas(SKCanvas canvas) {
        Vector2 rotationPos = new(PivotPosition.X + Size.X * RotationOffset.X,
            PivotPosition.Y + Size.Y * RotationOffset.Y);
        canvas.RotateDegrees(LocalRotation, rotationPos.X, rotationPos.Y);
        canvas.Translate(PivotPosition.X, PivotPosition.Y);
    }
    private void ValueChanged() {
        UpdateMatrix();
        DoValueChanged?.Invoke();
        Element?.TransformValueChanged();
    }
    
    public void UpdateMatrix() {
        SKMatrix matrix;
        
        if (Element?.Parent == null)
            matrix = SKMatrix.CreateIdentity();
        else
            matrix = Element.Parent.Transform.Matrix;
        
        var position = LocalPosition - OffsetPosition * Size + (Element?.Parent?.Transform.Size ?? new Vector2(0, 0)) * AnchorPosition;
        
        var mappedPosition = matrix.MapPoint(position.X, position.Y);
        // canvas.DrawPoint(mappedPosition, new SKPaint { Color = GetHashCodeColor(), StrokeWidth = 10 });
        
        matrix = SKMatrix.CreateIdentity();
        matrix = matrix.PostConcat(SKMatrix.CreateTranslation(mappedPosition.X, mappedPosition.Y));
        
        var point2 = matrix.MapPoint(new(0, 0));
        matrix = matrix.PostConcat(SKMatrix.CreateRotationDegrees(Element?.Parent?.Transform.WorldRotation ?? 0, point2.X, point2.Y));
        
        var point = new SKPoint(RotationOffset.X * Size.X, RotationOffset.Y * Size.Y);
        point = matrix.MapPoint(point);
        matrix = matrix.PostConcat(SKMatrix.CreateRotationDegrees(LocalRotation, point.X, point.Y));
        
        Matrix = matrix;
    }
    
    #region Size
    
    private Vector2 ParentSize {
        get => Element?.Parent?.Transform.Size ?? Vector2.Zero;
    }
    private Vector2 ParentScaleSize {
        get => ParentSize * ParentScale;
    }
    
    public Vector2 ParentScale {
        get => _parentScale;
        set {
            _parentScale = value;
            UpdateSize();
        }
    }
    
    private Vector2 _parentScale = Vector2.Zero;
    
    public bool ScaleWidth {
        get => ParentScale.X != 0;
        set => ParentScale = new(value ? 1 : 0, ParentScale.Y);
    }
    
    public bool ScaleHeight {
        get => ParentScale.Y != 0;
        set => ParentScale = new(ParentScale.X, value ? 1 : 0);
    }
    
    public Vector2 LocalSizeOffset {
        get => _localSizeOffset;
        set {
            _localSizeOffset = value;
            UpdateSize();
        }
    }
    
    private Vector2 _localSizeOffset = Vector2.Zero;
    
    public Vector2 Size {
        get => _size;
        set {
            if (ParentScale.X != 0) _localSizeOffset.X = value.X - ParentScaleSize.X;
            if (ParentScale.Y != 0) _localSizeOffset.Y = value.Y - ParentScaleSize.Y;
            _size = value;
            
            ExecuteChildren(child => child.Transform.UpdateSize());
            UpdatePosition();
        }
    }
    
    private Vector2 _size = Vector2.Zero;
    
    public Action<Vector2> ProcessSizeChanged;
    
    public void UpdateSize() {
        if (ParentScale.X != 0) _size.X = ParentScaleSize.X + _localSizeOffset.X;
        if (ParentScale.Y != 0) _size.Y = ParentScaleSize.Y + _localSizeOffset.Y;
        ProcessSizeChanged?.Invoke(_size);
        ValueChanged();
        ExecuteChildren(child => child.Transform.UpdateSize());
    }
    
    #endregion
    
    #region Position
    
    private Vector2 ParentPosition {
        get => Element?.Parent?.Transform.WorldPosition ?? Vector2.Zero;
    }
    
    public Vector2 LocalPosition {
        get => _localPosition;
        set {
            _localPosition = value;
            UpdatePosition();
        }
    }
    
    private Vector2 _localPosition = Vector2.Zero;
    
    public Vector2 AnchorPosition {
        get => _anchorPosition;
        set {
            _anchorPosition = value;
            UpdatePosition();
        }
    }
    
    private Vector2 _anchorPosition = Vector2.Zero;
    
    public Vector2 OffsetPosition {
        get => _offsetPosition;
        set {
            _offsetPosition = value;
            UpdatePosition();
        }
    }
    
    private Vector2 _offsetPosition = Vector2.Zero;
    
    public Vector2 WorldPosition {
        get => _worldPosition;
        set {
            _worldPosition = value;
            LocalPosition = _worldPosition - ParentPosition -
                            (ParentSize * AnchorPosition - Size * OffsetPosition);
        }
    }
    
    private Vector2 _worldPosition = Vector2.Zero;
    
    public Vector2 PivotPosition {
        get => _pivotPosition;
        set {
            LocalPosition = value - (ParentSize * AnchorPosition - Size * OffsetPosition);
            UpdatePosition();
        }
    }
    
    private Vector2 _pivotPosition = Vector2.Zero;
    
    public Action<Vector2> ProcessPositionChanged;
    
    public void UpdatePosition() {
        _pivotPosition = ParentSize * AnchorPosition - Size * OffsetPosition + LocalPosition;
        _worldPosition = ParentPosition + _pivotPosition;
        ProcessPositionChanged?.Invoke(_worldPosition);
        ValueChanged();
        ExecuteChildren(child => child.Transform.UpdatePosition());
    }
    
    #endregion
    
    #region Rotation
    
    private float ParentRotation {
        get => Element?.Parent?.Transform.WorldRotation ?? 0;
    }
    
    public float LocalRotation {
        get => _localRotation;
        set {
            _localRotation = value;
            UpdateRotation();
        }
    }
    
    private float _localRotation;
    
    public float WorldRotation {
        get => _worldRotation;
        set {
            _worldRotation = value;
            LocalRotation = _worldRotation - ParentRotation;
        }
    }
    
    private float _worldRotation;
    
    public Action<float> ProcessRotationChanged;
    
    public void UpdateRotation() {
        _worldRotation = ParentRotation + _localRotation;
        ProcessRotationChanged?.Invoke(_worldRotation);
        ValueChanged();
        ExecuteChildren(child => child.Transform.UpdateRotation());
    }
    
    #endregion
}
