using System.Diagnostics;
using SkiaSharp;
using Xunit.Abstractions;
using Yoru.Elements;
using Yoru.Graphics;

namespace Yoru.Tests;

[ApplicationTest]
public class HeadlessTests {
    [Fact]
    public void CreateHeadless() {
        Application app = ApplicationInstance.GetApplication();
        Assert.True(app.Renderer.Surface is null);
        app.Load();
        Assert.True(app.Renderer.Surface is not null);
    }
    
    [Fact]
    public void TrialCanvasScale() {
        Application app = ApplicationInstance.GetApplication();
        BoxElement box = new() {
            Color = SKColors.Red,
            Transform = new() {
                Size = new(100),
                AnchorPosition = new(0.5f),
                OffsetPosition = new(0.5f)
            }
        };
        
        app.Element.AddChild(box);
        
        app.CanvasScale = new(2);
        app.Load();

        app.CanvasScale = new(4);
        Assert.Equal(new(4), app.CanvasScale);

        app.Resize(1920, 1080);

        app.CanvasScale = new(6);
        app.Render();
        Assert.Equal(1920 / 6, (int)app.Element.Transform.Size.X);

        app.CanvasScale = new(8);
        app.Render();
        Assert.Equal(1920 / 8, (int)app.Element.Transform.Size.X);

        Assert.Equal(1920, app.FramebufferSize.X);
    }
}
