using System.Numerics;
using SkiaSharp;

namespace Yoru.Elements;

public class PathElement : ColorableElement {
    private SKPath _drawPath = new();
    public bool AutoResize = true;
    
    public SKPath DrawPath {
        get => _drawPath;
        set {
            _drawPath = value;
            if (AutoResize)
                Transform.Size = new(value.Bounds.Width, value.Bounds.Height);
        }
    }
    
    public Vector2[] Points {
        set {
            // get the lowest point (smallest x and y)
            var minX = value[0].X;
            var minY = value[0].Y;
            
            for (var i = 1; i < value.Length; i++) {
                if (value[i].X < minX)
                    minX = value[i].X;
                
                if (value[i].Y < minY)
                    minY = value[i].Y;
            }
            
            DrawPath.Reset();
            DrawPath.MoveTo(value[0].X - minX, value[0].Y - minY);
            
            for (var i = 1; i < value.Length; i++)
                DrawPath.LineTo(value[i].X - minX, value[i].Y - minY);
            
            DrawPath.Close();
            
            if (AutoResize) {
                Transform.Size = new(DrawPath.Bounds.Width, DrawPath.Bounds.Height);
                Transform.LocalPosition = new(minX, minY);
            }
        }
    }
    
    public SKPathEffect PathEffect {
        get => Paint.PathEffect;
        set => Paint.PathEffect = value;
    }
    
    public SKPathFillType FillType {
        get => DrawPath.FillType;
        set => DrawPath.FillType = value;
    }
    
    public float StrokeWidth {
        get => Paint.StrokeWidth;
        set => Paint.StrokeWidth = value;
    }
    
    public SKStrokeCap StrokeCap {
        get => Paint.StrokeCap;
        set => Paint.StrokeCap = value;
    }
    
    public SKStrokeJoin StrokeJoin {
        get => Paint.StrokeJoin;
        set => Paint.StrokeJoin = value;
    }
    
    public SKPathMeasure PathMeasure {
        get => new(DrawPath);
    }
    
    protected override void OnRender(SKCanvas canvas) =>
        canvas.DrawPath(DrawPath, Paint);
}
