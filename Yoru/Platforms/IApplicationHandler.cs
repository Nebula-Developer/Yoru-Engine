
using System.Numerics;

namespace Yoru.Platforms;

public interface IApplicationHandler {
    public string Title { get; set; }
    public Vector2 Size { get; set; }
    public void Close();

    public double RenderFrequency { get; set; }
    public double UpdateFrequency { get; set; }
}
