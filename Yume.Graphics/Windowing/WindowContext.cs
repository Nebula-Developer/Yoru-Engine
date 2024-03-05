
namespace Yume.Graphics.Windowing;

public class WindowContext(Window? window) {
    /// <summary>
    /// The window this context is attached to
    /// </summary>
    public Window? Window { get; } = window;
}
