using Yoru.Graphics;

namespace Yoru.Elements;

public class GridElement : Element {
    private float _columnSpacing;
    private float _elementHeight = 100;
    private float _elementWidth = 100;
    private GridFlowDirection _flowDirection = GridFlowDirection.Column;
    private int _maxColumns = 1;
    private int _maxRows = 1;
    private float _rowSpacing;
    public int MaxRows {
        get => _maxRows;
        set {
            if (_maxRows == value) return;
            _maxRows = Math.Max(1, value);
            RemapGrid();
        }
    }
    
    public int MaxColumns {
        get => _maxColumns;
        set {
            if (_maxColumns == value) return;
            _maxColumns = Math.Max(1, value);
            RemapGrid();
        }
    }
    
    public float ElementWidth {
        get => _elementWidth;
        set {
            if (_elementWidth == value) return;
            _elementWidth = Math.Max(0, value);
            RemapGrid();
        }
    }
    
    public float ElementHeight {
        get => _elementHeight;
        set {
            if (_elementHeight == value) return;
            _elementHeight = Math.Max(0, value);
            RemapGrid();
        }
    }
    
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
            child.Transform.LocalPosition = new(column * (ElementWidth + ColumnSpacing), row * (ElementHeight + RowSpacing));
            if (FlowDirection == GridFlowDirection.Column) {
                column++;
                if (column >= MaxColumns) {
                    column = 0;
                    row++;
                }
            } else {
                row++;
                if (row >= MaxRows) {
                    row = 0;
                    column++;
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
}
