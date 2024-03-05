using OpenTK.Mathematics;

namespace Yume.Graphics.Elements;

public enum FlexDirection {
    Row,
    Column
}

public class FlexBox : Element {
    public float Margin {
        get => _margin;
        set {
            _margin = value;
            UpdateFlex();
        }
    }

    private float _margin = 0;

    public FlexDirection Direction {
        get => _direction;
        set {
            _direction = value;
            UpdateFlex();
        }
    }

    private FlexDirection _direction = FlexDirection.Column;

    public bool ResetAxis = true;

    private bool _updating = false;

    public void UpdateFlex() {
        _updating = true;

        float p = 0;
        for (int i = 0; i < Children.Count; i++) {
            Element child = Children[i];
            Vector2 reset = ResetAxis ? Vector2.Zero : child.Transform.LocalPosition;

            child.Transform.LocalPosition = new Vector2(
                Direction == FlexDirection.Row ? p : reset.X,
                Direction == FlexDirection.Column ? p : reset.Y
            );

            p += (Direction == FlexDirection.Row ? child.Transform.Size.X : child.Transform.Size.Y) + Margin;
        }

        _updating = false;
    }

    public void Process(Vector2 v) {
        if (_updating) return;
        UpdateFlex();
    }

    protected override void ChildAdded(Element child) {
        child.Transform.ProcessPositionChanged += Process;
        child.Transform.ProcessSizeChanged += Process;
        UpdateFlex();
    }

    protected override void ChildRemoved(Element child) {
        child.Transform.ProcessPositionChanged -= Process;
        child.Transform.ProcessSizeChanged -= Process;
        UpdateFlex();
    }
}