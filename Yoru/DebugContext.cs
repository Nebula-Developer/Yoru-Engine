namespace Yoru;

public class DebugContext(Application app) : AppContext(app) {
    public long FrameCount { get; private set; }
    public int RenderDepth { get; internal set; }

    public void Update() {
        FrameCount++;
        RenderDepth = 0;
    }
}
