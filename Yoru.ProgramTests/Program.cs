using System.Globalization;
using SkiaSharp;
using Yoru.Graphics.Elements;
using Yoru.Input;
using Yoru.Windowing;

namespace Yoru.ProgramTests;

public class ProgramTestWindow : Window {
    private Box _box = new();
    private SimpleText _updateText = new();
    private SimpleText _frequencyText = new();

    protected override void Load() {
        base.Load();
        _box.Color = SKColors.Green;
        _box.Transform.Size = new(100);
        _box.Transform.ScaleWidth = true;

        _updateText.Transform.AnchorPosition = new(0.5f);
        _frequencyText.Transform.AnchorPosition = new(0.5f);
        _updateText.Transform.OffsetPosition = new(1, 0.5f);
        _frequencyText.Transform.OffsetPosition = new(0, 0.5f);
        
        _updateText.Color = SKColors.Red;
        _frequencyText.Color = SKColors.Orange;
        
        _updateText.FontSize = 50;
        _frequencyText.FontSize = 50;

        Element.AddChild(_box);
        _box.AddChild(_updateText);
        _box.AddChild(_frequencyText);
        
        _frequencyText.Text = Frequency.ToString(CultureInfo.InvariantCulture);
    }

    private double freq = 60;

    protected override void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine("Multithreaded: " + IsMultiThreaded);
        }
        
        if (Input.GetKey(KeyCode.Up)) {
            freq += 100.0 * (RenderTime.DeltaTime);
            Frequency = (int)freq;
            _frequencyText.Text = Frequency.ToString(CultureInfo.InvariantCulture);
        }
        
        if (Input.GetKey(KeyCode.Down)) {
            freq -= 100.0 * (RenderTime.DeltaTime);
            Frequency = (int)freq;
            _frequencyText.Text = Frequency.ToString(CultureInfo.InvariantCulture);
        }

        _updateText.Text = (1 / UpdateTime.RawDeltaTime).ToString("0.0");
    }
}

public static class Program {
    public static void Main(string[] args) {
        ProgramTestWindow programTestWindow = new();
        programTestWindow.Run();
    }
}
