using SkiaSharp;
using Xunit.Abstractions;
using Yoru.Elements;

namespace Yoru.Tests;

public class HeadlessTests(ITestOutputHelper output) {
    [Fact]
    public void CreateHeadless() {
        Application app = new();
        Assert.True(app.Renderer.Surface is null);
        app.Load();
        Assert.True(app.Renderer.Surface is not null);
    }
    
    [Fact]
    public void TrialCanvasScale() {
        Application app = new();
        BoxElement box = new() {
            Color = SKColors.Red,
            Transform = new() {
                Size = new(100),
                AnchorPosition = new(0.5f),
                OffsetPosition = new(0.5f)
            }
        };
        
        app.Element.AddChild(box);
        
        app.CanvasScale = 2;
        app.Load();
        app.CanvasScale = 4;
        app.Resize(1920, 1080);
        app.CanvasScale = 6;
        app.Render();
        app.CanvasScale = 8;
        
        Assert.Equal(1920 / 8, app.Element.Transform.Size.X);
        Assert.Equal(1920, app.FramebufferSize.X);
    }
}
