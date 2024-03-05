using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using Yume.Windowing;
using Window = Yume.Windowing.Window;

namespace Yume.Input;

public class InputContext(Window? window) : WindowContext(window) {
    public List<KeyCode> Keys { get; private set; } = new();
    public KeyCode[] PriorKeys { get; private set; } = new KeyCode[0];

    public bool GetKeyDown(KeyCode key) => Keys.Contains(key) && !PriorKeys.Contains(key);
    public bool GetKeyUp(KeyCode key) => !Keys.Contains(key) && PriorKeys.Contains(key);
    public bool GetKey(KeyCode key) => Keys.Contains(key);

    public void KeyDown(Keys key) {
        KeyCode keyCode = (KeyCode)key;
        if (!Keys.Contains(keyCode))
            Keys.Add(keyCode);
    }

    public void KeyUp(Keys key) {
        KeyCode keyCode = (KeyCode)key;
        if (Keys.Contains(keyCode))
            Keys.Remove(keyCode);
    }

    public void Update() {
        PriorKeys = Keys.ToArray();
    }
}