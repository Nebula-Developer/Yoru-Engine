
using Yoru.Graphics;

namespace Yoru.Tests;

[ApplicationTest]
public class ApplicationExecutor {
    [Fact]
    public void SurfaceExists() {
        Application app = ApplicationInstance.GetApplication();
        app.Load();
        Assert.True(app.Renderer.Surface is not null);
    }

    [Fact]
    public void MouseInteraction() {
        Application app = ApplicationInstance.GetApplication();
        app.Load();

        Element elmA = new Element() {
            Transform = new() {
                Size = new(10)
            },
            ZIndex = 0,
            Parent = app.Element
        };

        Element elmB = new Element() {
            Transform = new() {
                Size = new(100),
                LocalPosition = new(10, 10)
            },
            ZIndex = 1,
            Parent = app.Element
        };

        app.Input.UpdateMousePosition(new(9.99f, 9.99f));
        Assert.True(elmA.IsMouseOver);
        Assert.False(elmB.IsMouseOver);

        app.Input.UpdateMousePosition(new(10.01f, 10.01f));
        Assert.True(elmB.IsMouseOver);
        Assert.False(elmA.IsMouseOver);
    }
}
