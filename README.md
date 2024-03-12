# 🌙 Yoru Engine

### C# Graphics Engine for simplified and efficient software development


## About

Yoru is a C# library that supplies a direct approach to single/multithreaded development with SkiaSharp, backed by the capabilities of OpenTK.

## Installation

You may find the Nuget package [Here.](https://www.nuget.org/packages/Yoru)

### .NET CLI
`dotnet add package Yoru --prerelease`

<sub>Or alternatively, add this to your .csproj..</sub>

`<PackageReference Include="Yoru" Version="0.1.0" />`

## Usage

More information and/or a wiki will be added later. For now, you can understand the basics from this demonstration below:

```cs
using Yoru.Windowing;
using Yoru.Graphics.Elements;
using SkiaSharp;
using OpenTK.Mathematics;

namespace Yoru.Example;

public class MyWindow : Window {
    private Box _myBox = new() {
        Color = SKColors.White,
        Transform = new() {
            Size = new(100, 100),

            // Anchor/Offset are relative to the parent element,
            // and are calculated based on the size of this element
            // and its parent. (0.5f = 50% of the size of the parent element,
            // therefore it will be centered)
            OffsetPosition = new(0.5f),
            AnchorPosition = new(0.5f),
            RotationOffset = new (0.5f)
        }
    };

    protected override void Load() {
        base.Load();
        // "Element" is the windows root element,
        //  where children are added in order to be active.
        Element.AddChild(_myBox);
    }

    protected override void Update() {
        base.Update();
        // There is both UpdateTime and RenderTime,
        // which are respective to their individual methods.
        _myBox.Transform.LocalRotation += (float)(100 * UpdateTime.DeltaTime);
    }

    private SKPaint _manualBoxPaint = new() {
        Color = SKColors.Orange
    };

    protected override void Render() {
        base.Render();
        // Manually rendering to the windows Canvas:
        Canvas.DrawRect(10, 10, 50, 50, _manualBoxPaint);
    }
}

// Entry point
public static class Program {
    public static void Main(string[] args) {
        using var window = new MyWindow();
        window.IsMultiThreaded = false;
        window.Run();
    }
}
```
