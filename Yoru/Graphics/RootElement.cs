namespace Yoru.Graphics;

public class RootElement : Element {
    public RootElement(Application app) {
        App = app;
        Transform.Size = app.Handler.Size;
    }
    
    protected override void OnResize(int width, int height) => Transform.Size = new(width, height);
}
