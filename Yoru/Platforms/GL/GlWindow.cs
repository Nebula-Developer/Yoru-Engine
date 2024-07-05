#nullable disable

using System.Numerics;

namespace Yoru.Platforms.GL;

public class GlWindow : IApplicationHandler {
    internal GlWindowBridge Bridge;
    
    public GlWindow() => Bridge = new();
    internal GlRenderer Renderer {
        get => Bridge.Renderer;
    }
    
    public Application App {
        get => Bridge.App;
        set => Bridge.App = value;
    }
    
    public double RenderFrequency {
        get => Bridge.RenderFrequency;
        set => Bridge.RenderFrequency = value;
    }
    
    public double UpdateFrequency {
        get => Bridge.UpdateFrequency;
        set => Bridge.UpdateFrequency = value;
    }
    
    public string Title {
        get => Bridge.Title;
        set => Bridge.Title = value;
    }
    
    public Vector2 Size {
        get => Bridge.Size;
    }

    public bool VSync {
        get => Bridge.VSync;
        set => Bridge.VSync = value;
    }
    
    public void Close() => Bridge.Close();
    public void Run() => Bridge.Run();
}
