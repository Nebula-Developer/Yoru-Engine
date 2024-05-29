using System.Numerics;

namespace Yoru.Platforms;

public class HeadlessHandler : IApplicationHandler {
    public string Title { get; set; } = "";
    public Vector2 Size { get; set; } = new(1);
    public void Close() { }
    public void Run() { }
    
    public double RenderFrequency { get; set; } = 0;
    public double UpdateFrequency { get; set; } = 0;
}
