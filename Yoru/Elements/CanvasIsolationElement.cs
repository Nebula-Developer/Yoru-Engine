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
    
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        Transform.ProcessSizeChanged += _ => ResizeSurface();
    }
    
    protected override void OnLoad() {
        base.OnLoad();
        ResizeSurface();
    }
    
    protected override void OnRenderChildren(SKCanvas canvas) {
        if (Passthrough) {
            base.OnRenderChildren(canvas);
            return;
        }
        
        if (ClearCanvas) Canvas.Clear();
        ForChildren(child => child.Render(Canvas));
        if (RenderCanvas) canvas.DrawSurface(Surface, 0, 0);
    }
}
