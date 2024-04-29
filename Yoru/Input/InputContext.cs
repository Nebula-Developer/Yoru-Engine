#nullable disable

using System.Numerics;
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.Input;

public class InputContext(Application app) : AppContext(app) {
    private readonly Dictionary<MouseButton, int> _pressedButtons = new();
    private readonly Dictionary<Key, int> _pressedKeys = new();
    private readonly Dictionary<MouseButton, int> _releasedButtons = new();
    private readonly Dictionary<Key, int> _releasedKeys = new();
    private HashSet<Key> _keys { get; } = new();
    private HashSet<MouseButton> _buttons { get; } = new();

    public IReadOnlyCollection<Key> Keys => _keys;
    public IReadOnlyCollection<MouseButton> Buttons => _buttons;
    public Vector2 MousePosition { get; private set; } = new();

    private List<Element> HoveredElements = new();
    private List<Element> InteractingElements = new();
    private Dictionary<MouseButton, List<Element>> PressedElements = new();
    public int MaskIndex = 0;
    
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
            if (element.PointIntersects(MousePosition)) {
                if (!element.ClickThrough) {
                    removingElements.AddRange(interactingElements);
                    interactingElements.Clear();
                }

                interactingElements.Add(element);
            } else removingElements.Add(element);

            queue.Enqueue(element);
        }

        for (int i = 0; i < removingElements.Count; i++) {
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
        
        for (int i = 0; i < completed.Count; i++) {
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

        List<Element> completedElements = new();
        for (int i = 0; i < PressedElements.Count; i++) {
            var list = PressedElements.ElementAt(i).Value;
            for (int j = 0; j < list.Count; j++) {
                if (completedElements.Contains(list[j]))
                    continue;
                completedElements.Add(list[j]);
                list[j].MouseMove(position);
            }
        }
    }
    
    public void UpdateElementTransform(Element element) => HandleMouseInteractions();
    
    public void HandleMouseDown(MouseButton button) {
        _buttons.Add(button);
        _pressedButtons.TryGetValue(button, out var count);
        _pressedButtons[button] = count + 1;

        if (!PressedElements.ContainsKey(button))
            PressedElements[button] = new();
        else
            PressedElements[button].Clear();
        
        for (int i = InteractingElements.Count - 1; i >= 0; i--) {
            var x = InteractingElements[i];
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

        for (int i = PressedElements[button].Count - 1; i >= 0; i--) {
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
