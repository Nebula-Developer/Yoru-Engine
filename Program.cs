#pragma warning disable CS0162

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

public class MyWindow : Window {
    private SimpleText FPSText = new();
    // public BoxElement Box = new();
    public BoxElement Box = new();


    public override void Load() {
        base.Load();

        Box.Color = SKColors.White;
        Box.Transform.Size = new(50);
        Element.AddChild(Box);

        FPSText.ZIndex = 100;
        FPSText.TextColor = SKColors.White;
        FPSText.FontSize = 20;
        FPSText.Transform.WorldPosition = new(100, 100);
        Element.AddChild(FPSText);
    }

    public override void Render() {
        base.Render();
        FPSText.Text = "FPS: " + (1 / RenderTime.DeltaTime).ToString("0.00");
    }

    public override void Update() {
        base.Update();
        Box.Transform.LocalRotation += 100 * UpdateTime.DeltaTime;
        Box.Transform.WorldPosition = MousePosition * 2;
    }

    public override void KeyDown(KeyboardKeyEventArgs e) {
        base.KeyDown(e);

        if (e.Key == Keys.Space) {
            IsMultiThreaded = !IsMultiThreaded;
            Console.WriteLine("Multithreaded: " + IsMultiThreaded);
        }

        else if (e.Key == Keys.Down) {
            UpdateFrequency -= 50;
            RenderFrequency -= 50;
            Console.WriteLine("Update/Render Frequency: " + UpdateFrequency);
        }

        else if (e.Key == Keys.Up) {
            UpdateFrequency += 50;
            RenderFrequency += 50;
            Console.WriteLine("Update/Render Frequency: " + UpdateFrequency);
        }
    }
}

public static class Program {
    public static string SetX(int x) => "\x1b[" + x + "G";

    public static string FormatLogs(params string[] logs) {
        string result = "";
        for (int i = 0; i < logs.Length; i++) {
            result += SetX(i * 30) + logs[i];
        }

        return result;
    }

    public static string LogElementSize(Element element) {
        return FormatLogs(
            "LocalOffset: " + element.Transform.LocalSizeOffset,
            "Size: " + element.Transform.Size,
            "ParentScale: " + element.Transform.ParentScale
        );
    }

    public static string CheckBoxTicked(bool value) => value ? "☑" : "☐";

    public static string LogElementPosition(Element element) {
        return FormatLogs(
            "LocalPosition: " + element.Transform.LocalPosition,
            "WorldPosition: " + element.Transform.WorldPosition,
            "AnchorPosition: " + element.Transform.AnchorPosition,
            "OffsetPosition: " + element.Transform.OffsetPosition,
            "PivotPosition: " + element.Transform.PivotPosition,
            "Size: " + element.Transform.Size
        );
    }

    public static List<Element> elements = new();

    public static void Main(string[] args) {
        MyWindow window = new();
        window.UpdateFrequency = 144;
        window.RenderFrequency = 144;

        window.Run();

        return;
        for (int i = 0; i < 10; i++) {
            Element elm = new();
            // elm.Transform.ParentScale = Vector2.One;
            elm.Parent = elements.Count > 0 ? elements[elements.Count - 1] : null;
            elements.Add(elm);
        }

        while (true) {
            Console.Clear();
            
            for (int i = 0; i < elements.Count; i++)
                Console.WriteLine(i + ": \n" + LogElementPosition(elements[i]));

            try {
                string s = Console.ReadLine() ?? "";

                if (s.Length < 5) continue;

                Element element = elements[int.Parse(s[0].ToString())];

                Vector2 value = new();
                string[] values = s.Substring(2).Split(',');
                if (values.Length < 2) continue;

                value.X = float.Parse(values[0]);
                value.Y = float.Parse(values[1]);

                switch (s[1]) {
                    case 'l':
                        element.Transform.LocalPosition = value;
                        break;
                    case 'w':
                        element.Transform.WorldPosition = value;
                        break;
                    case 'a':
                        element.Transform.AnchorPosition = value;
                        break;
                    case 'o':
                        element.Transform.OffsetPosition = value;
                        break;
                    case 'p':
                        element.Transform.PivotPosition = value;
                        break;
                    case 's':
                        element.Transform.Size = value;
                        break;
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
        
    }
}
