using SkiaSharp;
using Xunit.Abstractions;
using Yoru.Elements;

namespace Yoru.Tests;

public class HeadlessTests(ITestOutputHelper output) {
    [Fact]
    public void CreateHeadless() {
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

        app.Load();
        app.Handler.Size = new(1920, 1080);
        app.Resize(1920, 1080);
        app.Render();

        Assert.True(app.Renderer.Canvas is not null);
        Assert.True(box.Transform.WorldPosition.X == 910);
        Assert.True(app.Handler.Size == new System.Numerics.Vector2(1920, 1080));
    }
}