#nullable disable

using System.Numerics;
using Yoru.Graphics;

namespace Yoru.Input;

public class InputContext(Application app) : AppContext(app) {
    private readonly Dictionary<MouseButton, int> _pressedButtons = new();
    private readonly Dictionary<MouseButton, List<Element>> _pressedElements = new();
    private readonly Dictionary<Key, int> _pressedKeys = new();
    private readonly Dictionary<MouseButton, int> _releasedButtons = new();
    private readonly Dictionary<Key, int> _releasedKeys = new();
    public readonly Dictionary<Element, bool> HoveredElementBlocking = new();
    
    public readonly List<Element> HoveredElements = new();
    public List<Element> InteractingElements = new();
    public int MaskIndex = 0;
    private HashSet<Key> _keys { get; } = new();
    private HashSet<MouseButton> _buttons { get; } = new();
    
    public IReadOnlyCollection<Key> Keys {
        get => _keys;
    }
    public IReadOnlyCollection<MouseButton> Buttons {
        get => _buttons;
    }
    public Vector2 MousePosition { get; private set; }
    
    public void Update() {
        HandleMouseInteractions();
    }
    
    public void UpdateCollections() {
        UpdateCollection(_pressedKeys);
        UpdateCollection(_releasedKeys);
        UpdateCollection(_pressedButtons);
        UpdateCollection(_releasedButtons);
    }
    
    private void UpdateCollection<T>(Dictionary<T, int> collection) {
        var keysToRemove = new List<T>();
        
        for (var i = collection.Count - 1; i >= 0; i--) {
            var item = collection.ElementAt(i);
            collection[item.Key]--;
            
            if (collection[item.Key] <= 0)
                keysToRemove.Add(item.Key);
        }
        
        keysToRemove.ForEach(key => collection.Remove(key));
    }
    
    public void HandleKeyDown(Key key) {
        _keys.Add(key);
        _pressedKeys.TryGetValue(key, out var count);
        _pressedKeys[key] = count + 1;
    }
    
    public void HandleKeyUp(Key key) {
        _keys.Remove(key);
        _releasedKeys.TryGetValue(key, out var count);
        _releasedKeys[key] = count + 1;
    }
    
    public void GetPositionalElements(ref Queue<Element> queue, ref List<Element> interactingElements) {
        if (queue.Count == 0) return;
        
        var elm = queue.Dequeue();
        
        for (var i = 0; i < elm.Children.Count; i++) {
            if (elm.Children[i].PointIntersects(MousePosition)) {
                queue.Enqueue(elm.Children[i]);
                interactingElements.Add(elm.Children[i]);
            }
        }
        
        GetPositionalElements(ref queue, ref interactingElements);
    }
    
    public void HandleMouseInteractions(Element elm = null) {
        elm ??= App.Element;
        Queue<Element> interactQueue = new();
        interactQueue.Enqueue(elm);
        List<Element> completed = new();
        GetPositionalElements(ref interactQueue, ref completed);
        
        completed.Reverse();
        
        void referenceFromPoint(int point) {
            completed.RemoveRange(point + 1, completed.Count - point - 1);
        }
        
        for (var i = 0; i < completed.Count; i++) {
            if (!HoveredElements.Contains(completed[i])) {
                completed[i].MouseEnter();
                HoveredElements.Add(completed[i]);
                HoveredElementBlocking[completed[i]] = completed[i].MaskMouseEvents; // TODO: Replace with MouseEnter() boolean return
                if (!HoveredElementBlocking[completed[i]]) {
                    referenceFromPoint(i);
                    break;
                }
            } else if (!HoveredElementBlocking[completed[i]]) {
                referenceFromPoint(i);
                break;
            }
        }
        
        for (var i = 0; i < HoveredElements.Count; i++) {
            if (!completed.Contains(HoveredElements[i])) {
                HoveredElements[i].MouseLeave();
                if (HoveredElementBlocking.ContainsKey(HoveredElements[i]))
                    HoveredElementBlocking.Remove(HoveredElements[i]);
                HoveredElements.RemoveAt(i--);
            }
        }
        
        InteractingElements = completed;
    }
    
    public void UpdateMousePosition(Vector2 position) {
        MousePosition = position;
        HandleMouseInteractions();
        
        for (var i = 0; i < InteractingElements.Count; i++)
            InteractingElements[i].MouseMove(position);
    }
    
    public void HandleMouseDown(MouseButton button) {
        HandleMouseInteractions();
        _buttons.Add(button);
        _pressedButtons.TryGetValue(button, out var count);
        _pressedButtons[button] = count + 1;
        
        if (!_pressedElements.ContainsKey(button))
            _pressedElements[button] = new();
        else
            _pressedElements[button].Clear();
        
        for (var i = InteractingElements.Count - 1; i >= 0; i--) {
            var x = InteractingElements.ElementAt(i);
            _pressedElements[button].Add(x);
            x.MouseDown(button);
            if (!x.MaskMouseEvents) break;
        }
    }
    
    public void HandleMouseUp(MouseButton button) {
        _buttons.Remove(button);
        _releasedButtons.TryGetValue(button, out var count);
        _releasedButtons[button] = count + 1;
        
        if (!_pressedElements.ContainsKey(button))
            return;
        
        for (var i = _pressedElements[button].Count - 1; i >= 0; i--) {
            var x = _pressedElements[button][i];
            x.MouseUp(button);
        }
        
        _pressedElements[button].Clear();
    }
    
    public bool GetKey(Key key) => _keys.Contains(key);
    public bool GetKeyDown(Key key) => _pressedKeys.ContainsKey(key);
    public bool GetKeyUp(Key key) => _releasedKeys.ContainsKey(key);
    
    public bool GetMouseButton(MouseButton button) => _buttons.Contains(button);
    public bool GetMouseButtonDown(MouseButton button) => _pressedButtons.ContainsKey(button);
    public bool GetMouseButtonUp(MouseButton button) => _releasedButtons.ContainsKey(button);
}
