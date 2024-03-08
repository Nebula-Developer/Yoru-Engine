using System.Collections.Generic;
using OpenTK.Windowing.Common.Input;
using Yume.Windowing;
using KeyCode = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using Window = Yume.Windowing.Window;
using System.Collections.Generic;

namespace Yume.Input;

public class InputContext(Window window) : WindowContext(window) {
    private Dictionary<KeyCode, bool> _keys = new();
    private Dictionary<KeyCode, int> _keysDown = new();
    private Dictionary<KeyCode, int> _keysUp = new();

    public bool GetKeyDown(KeyCode key) => _keysDown.TryGetValue(key, out var value) && (value > 0);

    public bool GetKeyUp(KeyCode key) => _keysUp.TryGetValue(key, out var value) && (value > 0);

    public bool GetKey(KeyCode key) => _keys.TryGetValue(key, out var value) && value;

    public void KeyDown(KeyCode key) {
        _keys[key] = true;
        _keysDown[key] = _keysDown.TryGetValue(key, out var value) ? value + 1 : 1;
    }

    public void KeyUp(KeyCode key) {
        _keys[key] = false;
        _keysUp[key] = _keysUp.TryGetValue(key, out var value) ? value + 1 : 1;
    }

    public void Update() {
        foreach (var key in _keysDown.Keys) {
            if (_keysDown[key] > 0)
                _keysDown[key]--;
        }
        
        foreach (var key in _keysUp.Keys) {
            if (_keysUp[key] > 0)
                _keysUp[key]--;
        }
    }
}