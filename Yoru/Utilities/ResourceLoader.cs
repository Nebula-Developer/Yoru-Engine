namespace Yoru.Utilities;

public class ResourceLoader {
    public Dictionary<string, byte[]> Resources { get; } = new();
    
    public byte[]? LoadResource(string path) {
        if (Resources.TryGetValue(path, out var data))
            return data;
        return null;
    }
    
    public void LoadResource(string path, byte[] data) =>
        Resources[path] = data;
}
