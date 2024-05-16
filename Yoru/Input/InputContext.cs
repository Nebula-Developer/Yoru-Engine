#nullable disable

using System.Numerics;
using Yoru.Graphics;

namespace Yoru.Input;

public class InputContext(Application app) : AppContext(app) {
    private readonly Dictionary<MouseButton, int> _pressedButtons = new();
    private readonly Dictionary<Key, int> _pressedKeys = new();
    private readonly Dictionary<MouseButton, int> _releasedButtons = new();
    private readonly Dictionary<Key, int> _releasedKeys = new();
    
    internal readonly List<Element> HoveredElements = new();
    private readonly Dictionary<MouseButton, List<Element>> PressedElements = new();
    internal List<Element> InteractingElements = new();
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
    
    public void Update() { }
    
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
    
    public void HandleMouseInteractions(ref Queue<Element> queue, ref List<Element> interactingElements, int position = 0) {
        var elements = queue.Dequeue().Children;
        List<Element> removingElements = new();
        
        for (var i = 0; i < elements.Count; i++) {
            var element = elements[i];
            
            if (!element.MouseInteraction) continue;
            queue.Enqueue(element);
            if (!element.HandleMouseEvents) continue;
            
            if (element.PointIntersects(MousePosition)) {
                if (!element.MaskMouseEvents) {
                    removingElements.AddRange(interactingElements);
                    interactingElements.Clear();
                }
                
                interactingElements.Add(element);
            } else if (HoveredElements.Contains(element)) removingElements.Add(element);
        }
        
        for (var i = 0; i < removingElements.Count; i++) {
            if (PressedElements.Any(x => x.Value.Contains(removingElements[i]))) {
                interactingElements.Add(removingElements[i]);
            } else if (HoveredElements.Contains(removingElements[i])) {
                HoveredElements.Remove(removingElements[i]);
                removingElements[i].MouseLeave();
            }
        }
        
        if (queue.Count > 0) HandleMouseInteractions(ref queue, ref interactingElements, position);
    }
    
    public void HandleMouseInteractions(Element elm = null) {
        elm = elm ?? App.Element;
        Queue<Element> interactQueue = new();
        interactQueue.Enqueue(elm);
        List<Element> completed = new();
        HandleMouseInteractions(ref interactQueue, ref completed);
        
        for (var i = 0; i < completed.Count; i++) {
            if (!HoveredElements.Contains(completed[i])) {
                HoveredElements.Add(completed[i]);
                completed[i].MouseEnter();
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
        _buttons.Add(button);
        _pressedButtons.TryGetValue(button, out var count);
        _pressedButtons[button] = count + 1;
        
        if (!PressedElements.ContainsKey(button))
            PressedElements[button] = new();
        else
            PressedElements[button].Clear();
        
        for (var i = InteractingElements.Count - 1; i >= 0; i--) {
            var x = InteractingElements.ElementAt(i);
            PressedElements[button].Add(x);
            
            if (!x.MouseDown(button))
                break;
        }
    }
    
    public void HandleMouseUp(MouseButton button) {
        _buttons.Remove(button);
        _releasedButtons.TryGetValue(button, out var count);
        _releasedButtons[button] = count + 1;
        
        if (!PressedElements.ContainsKey(button))
            return;
        
        for (var i = PressedElements[button].Count - 1; i >= 0; i--) {
            var x = PressedElements[button][i];
            
            if (!x.MouseUp(button))
                break;
        }
        
        PressedElements[button].Clear();
    }
    
    public bool GetKey(Key key) => _keys.Contains(key);
    public bool GetKeyDown(Key key) => _pressedKeys.ContainsKey(key);
    public bool GetKeyUp(Key key) => _releasedKeys.ContainsKey(key);
    
    public bool GetMouseButton(MouseButton button) => _buttons.Contains(button);
    public bool GetMouseButtonDown(MouseButton button) => _pressedButtons.ContainsKey(button);
    public bool GetMouseButtonUp(MouseButton button) => _releasedButtons.ContainsKey(button);
}
