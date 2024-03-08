#pragma warning disable CS0162


using System.Reflection;
using OpenTK.Windowing.Common;
using SkiaSharp;
using Yume.Graphics.Elements;
using Yume.Input;
using Window = Yume.Windowing.Window;


namespace Yume;

public class InheritWindow : Window {
    public static readonly string InternalDll = Path.GetFullPath("../../../../Internal/bin/Debug/net8.0/Internal.dll");
    private Action<SKCanvas>? _internalRenderMethod;

    protected override void Load() {
        Console.WriteLine("Loading Internal.dll from " + InternalDll);

        Assembly internalAssembly = Assembly.LoadFrom(InternalDll);
        internalAssembly.GetType("Internal.Static")?.GetField("_window")?.SetValue(null, this);

        MethodInfo? staticRenderMethod = internalAssembly.GetType("Internal.Static")?.GetMethod("Render");
        if (staticRenderMethod != null) {
            _internalRenderMethod =
                (Action<SKCanvas>)Delegate.CreateDelegate(typeof(Action<SKCanvas>), null, staticRenderMethod);
        }

        // get the Internal.Inner class (which inherits from Element)
        Type? innerType = internalAssembly.GetType("Internal.Inner");
        if (innerType != null) {
            // create an instance of the Inner class
            dynamic inner = (dynamic)Activator.CreateInstance(innerType)!;
            inner.Parent = Element;
        }
    }


    protected override void Render() {
        base.Render();
        _internalRenderMethod!(Canvas);
    }

    protected override void Update() {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space)) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine(IsMultiThreaded);
        }
    }
}

public static class Program {
    public static void Main(string[] args) {
        InheritWindow window = new() {
            Frequency = 144
        };

        window.Run();
    }
}