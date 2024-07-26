

using Yoru;
using SkiaSharp;
using System.Reflection;
using Yoru.Elements;
using System.Numerics;

namespace Yoru.Tests;

public class TestApplication : Application {
    public List<Type> Tests = new();
    public Dictionary<string, Action> TestMethods = new();

    protected override void OnLoad() {

        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        foreach (Type type in types) {
            if (type.Namespace == "Yoru.Tests" && type.GetCustomAttribute<ApplicationTestAttribute>() is not null) {
                Tests.Add(type);
                Console.WriteLine($"Found test: {type.Name}");
            }
        }

        foreach (Type test in Tests) {
            MethodInfo[] methods = test.GetMethods();
            foreach (MethodInfo method in methods) {
                if (method.GetCustomAttribute<FactAttribute>() is not null) {
                    TestMethods.Add($"{test.Name}.{method.Name}", () => {
                        object instance = Activator.CreateInstance(test)!;
                        method.Invoke(instance, null);
                    });
                }
            }
        }
    }

    protected override void OnRender() {
        base.OnRender();

        for (int i = 0; i < TestMethods.Count; i++) {
            AppCanvas.DrawText(TestMethods.Keys.ElementAt(i), 10, 30 + i * 20, new SKPaint {
                Color = SKColors.White,
                TextSize = 20
            });
        }
    }
}
