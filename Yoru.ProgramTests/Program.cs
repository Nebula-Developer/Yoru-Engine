#nullable disable

using System.Globalization;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using Yoru.Graphics;

namespace Yoru.ProgramTests;

public class ProgramTestWindow() : GameWindow(GameWindowSettings.Default, NativeWindowSettings.Default) {
    
}

public static class Program {
    public static void Main(string[] args) {
        ProgramTestWindow programTestWindow = new();
        programTestWindow.Run();
    }
}
