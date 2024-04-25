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
        // var column = 0;
        // var row = 0;
        // foreach (var child in Children) {
        //     child.Transform.LocalPosition = new(column * (child.Transform.Size.X + ColumnSpacing), row * (child.Transform.Size.Y + RowSpacing));
        //     if (FlowDirection == GridFlowDirection.Column) {
        //         column++;
        //         if (column * (child.Transform.Size.X + ColumnSpacing) >= Transform.Size.X - child.Transform.Size.X - ColumnSpacing) {
        //             column = 0;
        //             row++;
        //         }
        //     } else {
        //         row++;
        //         if (row * (child.Transform.Size.Y + RowSpacing) >= Transform.Size.Y - child.Transform.Size.Y - RowSpacing) {
        //             row = 0;
        //             column++;
        //         }
        //     }
        // }
        float x = 0;
        float y = 0;
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (var child in Children) {
            child.Transform.LocalPosition = new(x, y);
            if (child.Transform.Size.X > maxWidth) maxWidth = child.Transform.Size.X;
            if (child.Transform.Size.Y > maxHeight) maxHeight = child.Transform.Size.Y;
            if (FlowDirection == GridFlowDirection.Column) {
                x += child.Transform.Size.X + ColumnSpacing;
                if (x >= Transform.Size.X - child.Transform.Size.X - ColumnSpacing) {
                    x = 0;
                    y += maxHeight + RowSpacing;
                    maxHeight = 0;
                }
            } else {
                y += child.Transform.Size.Y + RowSpacing;
                if (y >= Transform.Size.Y - child.Transform.Size.Y - RowSpacing) {
                    y = 0;
                    x += maxWidth + ColumnSpacing;
                    maxWidth = 0;
                }
            }
        }
    }
    
    protected override void OnChildAdded(Element child) {
        base.OnChildAdded(child);
        RemapGrid();
    }
    
    protected override void OnChildRemoved(Element child) {
        base.OnChildRemoved(child);
        RemapGrid();
    }
    
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        Transform.ProcessSizeChanged += size => RemapGrid();
    }
}
