
namespace Yoru.Tests;

public static class ApplicationInstance {
    public static Application? Instance { get; private set; }
    public static Application GetApplication() => Instance ?? new Application();
}
