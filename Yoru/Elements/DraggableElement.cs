#nullable disable

using System.Numerics;
using Yoru.Graphics;
using Yoru.Input;

namespace Yoru.Elements;

public class DraggableElement : Element {
    private MouseButton? curButton;
    
    private Vector2 mouseStart;
    private Vector2 startPos;
    public MouseButton Button { get; set; } = MouseButton.Left;
    
    protected override void OnLoad() {
        base.OnLoad();
    }
    
    protected override void OnMouseDown(MouseButton button) {
        if (button != Button) return;
        curButton = Button;
        base.OnMouseDown(button);
        mouseStart = App.Input.MousePosition;
        startPos = Transform.WorldPosition;
    }
    
    protected override void OnMouseUp(MouseButton button) {
        if (curButton == null || button != curButton) return;
        curButton = null;
        base.OnMouseUp(button);
    }
    
    protected override void OnMouseMove(Vector2 position) {
        if (curButton == null) return;
        base.OnMouseMove(position);
        Transform.WorldPosition = startPos + position - mouseStart;
    }
}