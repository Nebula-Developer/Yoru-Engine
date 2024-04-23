#nullable disable

using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Elements;

public class CanvasIsolationElement : Element {
    public SKSurface Surface { get; protected set; }
    public SKCanvas Canvas {
        get => Surface.Canvas;
    }
    
    public bool RenderCanvas { get; set; } = true;
    public bool ClearCanvas { get; set; } = true;
    public bool Passthrough { get; set; } = false;
    
    protected virtual void ResizeSurface() {
        Surface?.Dispose();
        Surface = SKSurface.Create(new SKImageInfo((int)Transform.Size.X, (int)Transform.Size.Y));
    }
    
    protected override void TransformChanged() {
        base.TransformChanged();
        Transform.ProcessSizeChanged += size => ResizeSurface();
    }
    
    protected override void Load() {
        base.Load();
        ResizeSurface();
    }
    
    protected override void RenderChildren(SKCanvas canvas) {
        if (Passthrough) {
            base.RenderChildren(canvas);
            return;
        }
        
        if (ClearCanvas) Canvas.Clear();
        ForChildren(child => child.RenderSelf(Canvas));
        if (RenderCanvas) canvas.DrawSurface(Surface, 0, 0);
    }
}
