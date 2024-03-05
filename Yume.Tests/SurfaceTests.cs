using System.Diagnostics;
using SkiaSharp;
using Xunit.Abstractions;

namespace Yume.Tests;

public class SurfaceTests(ITestOutputHelper output) {
    [Theory]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void TestQuickReject(int rounds) {
        var surface = SKSurface.Create(new SKImageInfo(1920, 1080));

        Stopwatch st = new();
        st.Start();

        for (var i = 0; i < rounds; i++) {
            SKRect rect = new(i, i, 1920, 1080);
            surface.Canvas.QuickReject(rect);
        }

        st.Stop();
        output.WriteLine("QuickReject (" + rounds + " rounds) took: " + st.ElapsedMilliseconds + "ms");
    }

    [Theory]
    [InlineData(100, 1000, 1000)]
    [InlineData(1000, 100, 100)]
    [InlineData(10000, 10, 10)]
    public void InstantiateSurface(int rounds, int width, int height) {
        Stopwatch st = new();
        st.Start();

        for (var i = 0; i < rounds; i++) {
            var surface = SKSurface.Create(new SKImageInfo(width, height));
            surface.Dispose();
        }

        st.Stop();
        output.WriteLine("Instantiating " + width + "x" + height + " surfaces (" + rounds + " rounds) took: " +
                         st.ElapsedMilliseconds + "ms");
    }

    [Theory]
    [InlineData(10000, false)]
    [InlineData(10000, true)]
    [InlineData(100000, false)]
    [InlineData(100000, true)]
    [InlineData(1000000, false)]
    [InlineData(1000000, true)]
    public void TestCulledDrawing(int rounds, bool culled) {
        var surface = SKSurface.Create(new SKImageInfo(1920, 1080));

        Stopwatch st = new();
        st.Start();

        for (var i = 0; i < rounds; i++) {
            if (culled && surface.Canvas.QuickReject(new SKRect(i, i, 100, 100)))
                continue;

            surface.Canvas.DrawRect(i, i, 100, 100, new SKPaint());
        }

        st.Stop();
        output.WriteLine("Drawing to canvas (" + rounds + " rounds) took: " + st.ElapsedMilliseconds + "ms");
    }

    [Theory]
    [InlineData(1000, 100, 100)]
    [InlineData(10000, 100, 100)]
    [InlineData(100000, 100, 100)]
    public void DrawText(int rounds, int width, int height) {
        var surface = SKSurface.Create(new SKImageInfo(1920, 1080));

        Stopwatch st = new();
        st.Start();

        for (var i = 0; i < rounds; i++)
            surface.Canvas.DrawText("Hello, world!", i, i, new SKPaint());

        st.Stop();
        output.WriteLine("Drawing text (" + rounds + " rounds) took: " + st.ElapsedMilliseconds + "ms");
    }
}