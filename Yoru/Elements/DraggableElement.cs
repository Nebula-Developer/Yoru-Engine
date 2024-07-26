#nullable disable

using System.Numerics;
using Yoru.Graphics;
using Yoru.Input;

namespace Yoru.Elements;

public class DraggableElement : Element {
    private MouseButton? _curButton;
    
    private Vector2 _mouseStart;
    private Vector2 _startPos;
    public MouseButton Button { get; set; } = MouseButton.Left;
    
    protected override bool OnMouseDown(MouseButton button) {
        if (button != Button) return false;
        _curButton = Button;
        base.OnMouseDown(button);
        _mouseStart = App.Input.MousePosition;
        _startPos = Transform.WorldPosition;
        return false;
    }
    
    protected override void OnMouseUp(MouseButton button) {
        if (_curButton == null || button != _curButton) return;
        _curButton = null;
        base.OnMouseUp(button);
    }
    
    protected override void OnMouseMove(Vector2 position) {
        if (_curButton == null) return;
        base.OnMouseMove(position);
        Transform.WorldPosition = _startPos + position - _mouseStart;
    }
}
