#nullable disable

using SkiaSharp;

namespace Yoru.Graphics;

public class ImageElement : Element {
    public SKImage Image {
        get => _image;
        set {
            _image = value;
            if (AutoResize)
                Transform.Size = new(_image.Width, _image.Height);
        }
    }
    private SKImage _image;

    public bool AutoResize = true;

    public SKPaint Paint = new();

    public ImageElement(SKImage image) => Image = image;
    public ImageElement(string path) => Image = SKImage.FromEncodedData(path);

    protected override void Render(SKCanvas canvas) {
        if (Image != null)
            canvas.DrawImage(Image, new SKRect(0, 0, Transform.Size.X, Transform.Size.Y), Paint);
    }
}
