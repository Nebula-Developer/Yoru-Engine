using OpenTK.Mathematics;
using Yume.Graphics.Windowing;

namespace Yume.Graphics.Elements;

public class RootElement : Element {
    public RootElement(Window window) {
        this.Window = window;
        Transform.Size = window.Size;
    }

    public override void Resize(Vector2 size) => Transform.Size = size;
}