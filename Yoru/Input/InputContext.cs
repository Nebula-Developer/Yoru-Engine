#nullable disable

using System.Numerics;

namespace Yoru.Input;

public class InputContext(Application app) : AppContext(app) {
    private readonly Dictionary<MouseButton, int> _pressedButtons = new();
    private readonly Dictionary<Key, int> _pressedKeys = new();
    private readonly Dictionary<MouseButton, int> _releasedButtons = new();
    private readonly Dictionary<Key, int> _releasedKeys = new();
    
    public HashSet<Key> Keys { get; } = new();
    public HashSet<MouseButton> Buttons { get; } = new();
    public Vector2 MousePosition { get; private set; }
    
    public void UpdateMousePosition(Vector2 position) => MousePosition = position;
    
    public void Update() {
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
    }
    
    public void HandleMouseUp(MouseButton button) {
        Buttons.Remove(button);
        _releasedButtons.TryGetValue(button, out var count);
        _releasedButtons[button] = count + 1;
    }
    
    public bool GetKey(Key key) => Keys.Contains(key);
    public bool GetKeyDown(Key key) => _pressedKeys.ContainsKey(key);
    public bool GetKeyUp(Key key) => _releasedKeys.ContainsKey(key);
    
    public bool GetMouseButton(MouseButton button) => Buttons.Contains(button);
    public bool GetMouseButtonDown(MouseButton button) => _pressedButtons.ContainsKey(button);
    public bool GetMouseButtonUp(MouseButton button) => _releasedButtons.ContainsKey(button);
}
