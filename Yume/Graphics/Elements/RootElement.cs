using OpenTK.Mathematics;
using Yume.Windowing;

namespace Yume.Graphics.Elements;

public class RootElement : Element {
    public RootElement(Window window) {
        Window = window;
        Transform.Size = window.Size;
    }

    protected override void Resize(Vector2 size) => Transform.Size = size;
}