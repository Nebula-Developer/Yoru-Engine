
using System.Numerics;

namespace Yoru.Platforms;

public interface IApplicationHandler {
    public void SetTitle(string title);
    public void SetSize(int width, int height);
    public Vector2 Size { get; }
    public void Close();

    public double RenderFrequency { get; set; }
    public double UpdateFrequency { get; set; }
}
