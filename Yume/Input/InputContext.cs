using Yume.Windowing;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using Window = Yume.Windowing.Window;

namespace Yume.Input;

public class InputContext(Window? window) : WindowContext(window) {
    public List<KeyCode> Keys { get; } = new();
    public KeyCode[] PriorKeys { get; private set; } = new KeyCode[0];

    public bool GetKeyDown(KeyCode key) {
        return Keys.Contains(key) && !PriorKeys.Contains(key);
    }

    public bool GetKeyUp(KeyCode key) {
        return !Keys.Contains(key) && PriorKeys.Contains(key);
    }

    public bool GetKey(KeyCode key) {
        return Keys.Contains(key);
    }

    public void KeyDown(Keys key) {
        var keyCode = (KeyCode)key;
        if (!Keys.Contains(keyCode))
            Keys.Add(keyCode);
    }

    public void KeyUp(Keys key) {
        var keyCode = (KeyCode)key;
        if (Keys.Contains(keyCode))
            Keys.Remove(keyCode);
    }

    public void Update() {
        PriorKeys = Keys.ToArray();
    }
}