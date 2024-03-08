using System;
using System.Diagnostics;
using OpenTK.Mathematics;
using Xunit.Abstractions;
using Yoru.Graphics;
using Yoru.Graphics.Elements;
using Yoru.Windowing;

namespace Yoru.Tests;

public class ElementTests(ITestOutputHelper output) {
    [Fact]
    public void ParentChildrenBehaviour() {
        Element parent = new();
        Element child = new() {
            Parent = parent
        };

        Assert.Equal(parent, child.Parent);

        child.Transform.ScaleWidth = true;
        parent.Transform.Size = new Vector2(100);
        Assert.Equal(100, child.Transform.Size.X);

        parent.Transform.WorldRotation = 50;
        child.Transform.LocalRotation = 50;
        Assert.Equal(100, child.Transform.WorldRotation);

        child.Transform.ScaleWidth = false;
        child.Transform.Size = new Vector2(50, 50);
        child.Transform.AnchorPosition = new Vector2(0.5f);
        child.Transform.OffsetPosition = new Vector2(0.5f);
        Assert.Equal(new Vector2(25), child.Transform.PivotPosition);
    }

    [Fact]
    public void CircularReferenceException() {
        Element parent = new();
        Element child = new();
        parent.Parent = child;

        Assert.Throws<CircularElementReferenceException>(() => child.Parent = parent);
    }

    [Theory]
    [InlineData(100000)]
    [InlineData(1000000)]
    [InlineData(10000000)]
    public void TimeContext(int rounds) {
        TimeContext n = new(null);

        Stopwatch sw = new();
        sw.Start();

        n.TimeScale = 0.0001;
        for (var i = 0; i < rounds; i++)
            n.Update(0.001);

        sw.Stop();

        Assert.True((int)Math.Round(n.Time) == (int)Math.Round(n.TimeScale * (rounds * 0.001)));
        output.WriteLine("Time context updating (" + rounds + " rounds) took: " + sw.ElapsedMilliseconds + "ms");
    }
}