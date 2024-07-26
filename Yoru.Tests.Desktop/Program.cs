using Yoru.Platforms.GL;

namespace Yoru.Tests;

public static class Program {
    public static void Main(string[] args) {
        new GlWindow {
            App = new TestApplication()
        }.Run();
    }
}
