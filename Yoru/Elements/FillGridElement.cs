using Yoru.Graphics;

namespace Yoru.Elements;

public class FillGridElement : Element {
    private float _columnSpacing;
    private GridFlowDirection _flowDirection = GridFlowDirection.Column;
    private float _rowSpacing;
    public float RowSpacing {
        get => _rowSpacing;
        set {
            if (_rowSpacing == value) return;
            _rowSpacing = value;
            RemapGrid();
        }
    }
    
    public float ColumnSpacing {
        get => _columnSpacing;
        set {
            if (_columnSpacing == value) return;
            _columnSpacing = value;
            RemapGrid();
        }
    }
    
    public GridFlowDirection FlowDirection {
        get => _flowDirection;
        set {
            if (_flowDirection == value) return;
            _flowDirection = value;
            RemapGrid();
        }
    }
    
    public void RemapGrid() {
        var column = 0;
        var row = 0;
        foreach (var child in Children) {
            child.Transform.LocalPosition = new(column * (child.Transform.Size.X + ColumnSpacing), row * (child.Transform.Size.Y + RowSpacing));
            if (FlowDirection == GridFlowDirection.Column) {
                column++;
                if (column * (child.Transform.Size.X + ColumnSpacing) >= Transform.Size.X - child.Transform.Size.X - ColumnSpacing) {
                    column = 0;
                    row++;
                }
            } else {
                row++;
                if (row * (child.Transform.Size.Y + RowSpacing) >= Transform.Size.Y - child.Transform.Size.Y - RowSpacing) {
                    row = 0;
                    column++;
                }
            }
        }
    }
    
    protected override void ChildAdded(Element child) {
        base.ChildAdded(child);
        RemapGrid();
    }
    
    protected override void ChildRemoved(Element child) {
        base.ChildRemoved(child);
        RemapGrid();
    }
    
    protected override void TransformChanged() {
        base.TransformChanged();
        Transform.ProcessSizeChanged += size => RemapGrid();
    }
}
