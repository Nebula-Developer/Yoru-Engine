#nullable disable

using System.Reflection;
using SkiaSharp;
using Yoru.Elements;
using Yoru.Graphics;
using Yoru.Input;
using Yoru.Mathematics;
using Yoru.Platforms.GL;

namespace Yoru.ProgramTests;

public static class Program {
    public static void Main(string[] args) {
        GLWindow programTestWindow = new();
        Console.CancelKeyPress += (s, e) => { programTestWindow.Close(); e.Cancel = true; };
        programTestWindow.app = new EasingTestApp();
        programTestWindow.Run();
        Console.WriteLine("Program killed");
        programTestWindow.Dispose();
    }
}
