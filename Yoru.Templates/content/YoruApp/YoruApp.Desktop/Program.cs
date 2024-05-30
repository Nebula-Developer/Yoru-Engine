using Yoru.Platforms.GL;

namespace YoruApp.Desktop;

public static class Program {
    public static void Main(string[] args) {
        new GlWindow {
            App = new YoruAppApplication()
        }.Run();
    }
}
