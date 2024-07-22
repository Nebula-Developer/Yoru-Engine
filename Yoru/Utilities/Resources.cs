using System.Reflection;
using System.Text;

namespace Yoru.Utilities;

public static class Resources {
    public static ResourceLoader LocalData { get; } = new();        // For resources that are loaded from the filesystem
    public static ResourceLoader YoruResourceData { get; } = new(); // For resources that are embedded in Yoru's assembly
    public static ResourceLoader ResourceData { get; } = new();     // For resources that are embedded in the calling assembly
    
    public static Assembly YoruAssembly {
        get => Assembly.GetExecutingAssembly();
    }
    
    public static string? LoadLocalFileS(string path) {
        if (LoadLocalFile(path) is { } data)
            return Encoding.UTF8.GetString(data);
        return null;
    }
    
    public static byte[]? LoadLocalFile(string path) {
        if (LocalData.LoadResource(path) is { } data)
            return data;
        
        if (!File.Exists(path))
            return null;
        
        var file = File.ReadAllBytes(path);
        LocalData.LoadResource(path, file);
        return file;
    }
    
    public static byte[]? LoadYoruResourceFile(string path) {
        if (YoruResourceData.LoadResource(path) is { } data)
            return data;
        
        path = path.Replace("/", ".");
        path = YoruAssembly.GetName().Name + "." + path;
        
        var stream = YoruAssembly.GetManifestResourceStream(path);
        if (stream is null)
            return null;
        
        using var reader = new StreamReader(stream);
        var file = Encoding.UTF8.GetBytes(reader.ReadToEnd());
        LocalData.LoadResource(path, file);
        return file;
    }
    
    public static string? LoadYoruResourceFileS(string path) {
        if (LoadYoruResourceFile(path) is { } data)
            return Encoding.UTF8.GetString(data);
        return null;
    }
    
    public static byte[]? LoadResourceFile(string path) {
        if (ResourceData.LoadResource(path) is { } data)
            return data;
        
        var assembly = Assembly.GetCallingAssembly();
        path = path.Replace("/", ".");
        path = assembly.GetName().Name + "." + path;
        
        var stream = assembly.GetManifestResourceStream(path);
        if (stream is null)
            return null;
        
        using var reader = new StreamReader(stream);
        var file = Encoding.UTF8.GetBytes(reader.ReadToEnd());
        LocalData.LoadResource(path, file);
        return file;
    }
    
    public static string? LoadResourceFileS(string path) {
        if (LoadResourceFile(path) is { } data)
            return Encoding.UTF8.GetString(data);
        return null;
    }
}
