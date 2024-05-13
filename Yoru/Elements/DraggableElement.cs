#nullable disable

using System.Numerics;
using Yoru.Graphics;
using Yoru.Input;

namespace Yoru.ProgramTests;

public class DraggableElement : Element {
    private MouseButton? curButton;

    private Vector2 mouseStart;
    private Vector2 startPos;
    public MouseButton Button { get; set; } = MouseButton.Left;

    protected override void OnLoad() {
        base.OnLoad();
    }

    protected override bool OnMouseDown(MouseButton button) {
        if (button != Button) return true;
        curButton = Button;
        base.OnMouseDown(button);
        mouseStart = App.Input.MousePosition;
        startPos = Transform.WorldPosition;
        return true;
    }

    protected override bool OnMouseUp(MouseButton button) {
        if (curButton == null || button != curButton) return true;
        curButton = null;
        base.OnMouseUp(button);
        return true;
    }

    protected override void OnMouseMove(Vector2 position) {
        if (curButton == null) return;
        base.OnMouseMove(position);
        Transform.WorldPosition = startPos + position - mouseStart;
    }
}
