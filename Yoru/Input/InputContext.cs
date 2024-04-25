#nullable disable

using System.Numerics;
using Yoru.Graphics;

namespace Yoru.Input;

public class InputContext(Application app) : AppContext(app) {
    private readonly Dictionary<MouseButton, int> _pressedButtons = new();
    private readonly Dictionary<Key, int> _pressedKeys = new();
    private readonly Dictionary<MouseButton, int> _releasedButtons = new();
    private readonly Dictionary<Key, int> _releasedKeys = new();
    
    private readonly List<Element> HoveredElements = new(); // Elements that are under the mouse currently
    private List<Element> OldHoveredElements = new();
    private List<Element> PressedElements = new(); // Elements that were under the mouse when it was pressed
    
    public HashSet<Key> Keys { get; } = new();
    public HashSet<MouseButton> Buttons { get; } = new();
    public Vector2 MousePosition { get; private set; }
    
    public void Update() {
        OldHoveredElements = HoveredElements.ToList();
        HoveredElements.Clear();
    }
    
    public void UpdateCollections() {
        UpdateCollection(_pressedKeys);
        UpdateCollection(_releasedKeys);
        UpdateCollection(_pressedButtons);
        UpdateCollection(_releasedButtons);
    }
    
    private void UpdateCollection<T>(Dictionary<T, int> collection) {
        var keysToRemove = new List<T>();
        
        foreach (var item in collection) {
            collection[item.Key]--;
            
            if (collection[item.Key] <= 0)
                keysToRemove.Add(item.Key);
        }
        
        keysToRemove.ForEach(key => collection.Remove(key));
    }
    
    public void UpdateMousePosition(Vector2 position) {
        MousePosition = position;
        for (var i = HoveredElements.Count - 1; i >= 0; i--) {
            var element = HoveredElements[i];
            element.MouseMove(position);
            if (Buttons.Count > 0)
                element.MouseDrag();
        }
    }
    
    public void HandleElementEnter(Element element) {
        HoveredElements.Insert(0, element);
        if (!OldHoveredElements.Contains(element))
            element.MouseEnter();
    }
    
    public void HandleElementLeave(Element element) {
        HoveredElements.Remove(element);
        if (OldHoveredElements.Contains(element))
            element.MouseLeave();
    }
    
    public void HandleKeyDown(Key key) {
        Keys.Add(key);
        _pressedKeys.TryGetValue(key, out var count);
        _pressedKeys[key] = count + 1;
    }
    
    public void HandleKeyUp(Key key) {
        Keys.Remove(key);
        _releasedKeys.TryGetValue(key, out var count);
        _releasedKeys[key] = count + 1;
    }
    
    public void HandleMouseDown(MouseButton button) {
        Buttons.Add(button);
        _pressedButtons.TryGetValue(button, out var count);
        _pressedButtons[button] = count + 1;
        
        foreach (var element in HoveredElements)
            if (!element.MouseDown(button))
                break;
        
        PressedElements = HoveredElements.ToList();
    }
    
    public void HandleMouseUp(MouseButton button) {
        Buttons.Remove(button);
        _releasedButtons.TryGetValue(button, out var count);
        _releasedButtons[button] = count + 1;
        
        foreach (var element in PressedElements)
            if (!element.MouseUp(button))
                break;
    }
    
    public bool GetKey(Key key) => Keys.Contains(key);
    public bool GetKeyDown(Key key) => _pressedKeys.ContainsKey(key);
    public bool GetKeyUp(Key key) => _releasedKeys.ContainsKey(key);
    
    public bool GetMouseButton(MouseButton button) => Buttons.Contains(button);
    public bool GetMouseButtonDown(MouseButton button) => _pressedButtons.ContainsKey(button);
    public bool GetMouseButtonUp(MouseButton button) => _releasedButtons.ContainsKey(button);
}
