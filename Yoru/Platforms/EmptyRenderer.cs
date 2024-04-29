#nullable disable

using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Platforms;

public class EmptyRenderer : Renderer {
    public override void Resize(int width, int height) {
        base.Resize(width, height);
        width = Math.Max(1, width);
        height = Math.Max(1, height);
        _surface?.Dispose();
        _surface = SKSurface.Create(new SKImageInfo(width, height));
    }
    
    public override void Flush() {
        base.Flush();
        Canvas.Flush();
    }
    
    public void WriteToFile(string path) {
        using var stream = File.Open(path, FileMode.OpenOrCreate);
        _surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
}
