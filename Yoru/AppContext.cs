
namespace Yoru;

public class AppContext(Application app) {
    public Application App = app;
    public virtual void Load() { }
}
