using System.Reflection;
using System.Text;

namespace Yoru.Utilities;

public static class Resources {
    public static ResourceLoader Data { get; } = new();
    public static ResourceLoader ResourceData { get; } = new();
    public static Assembly YoruAssembly {
        get => Assembly.GetExecutingAssembly();
    }
    
    public static string? LoadFileS(string path) {
        if (LoadFile(path) is { } data)
            return Encoding.UTF8.GetString(data);
        return null;
    }
    
    public static byte[]? LoadFile(string path) {
        if (Data.LoadResource(path) is byte[] data)
            return data;
        
        if (!File.Exists(path))
            return null;
        
        var file = File.ReadAllBytes(path);
        Data.LoadResource(path, file);
        return file;
    }
    
    public static byte[]? LoadResourceFile(string path) {
        if (ResourceData.LoadResource(path) is { } data)
            return data;
        
        path = path.Replace("/", ".");
        path = YoruAssembly.GetName().Name + "." + path;
        
        var stream = YoruAssembly.GetManifestResourceStream(path);
        if (stream is null)
            return null;
        
        using var reader = new StreamReader(stream);
        var file = Encoding.UTF8.GetBytes(reader.ReadToEnd());
        Data.LoadResource(path, file);
        return file;
    }
    
    public static string? LoadResourceFileS(string path) {
        if (LoadResourceFile(path) is { } data)
            return Encoding.UTF8.GetString(data);
        return null;
    }
}