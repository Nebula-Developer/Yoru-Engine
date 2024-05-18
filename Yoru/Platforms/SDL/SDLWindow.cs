#nullable disable

using System.Numerics;
using Silk.NET.SDL;
using Silk.NET.OpenGL;
using SkiaSharp;
using Yoru.Input;
using System.Diagnostics;
using SilkKey = Silk.NET.Input.Key;
using Silk.NET.GLFW;
using Silk.NET.Input.Glfw;

namespace Yoru.Platforms.SDL;

using GL = Silk.NET.OpenGL.GL;
using MouseButton = Yoru.Input.MouseButton;

public unsafe class SDLWindow : IApplicationHandler {
    private float _dpi = 1;
    public Application App;
    public SDLRenderer Renderer;
    public SDLWindow() { }

    public bool Open { get; set; } = true;
    public void Close() => Open = false;

    public Window* Window { get; private set; }
    public void* Context { get; private set; }
    public Sdl SDL { get; private set; } = Sdl.GetApi();
    public GL GLA { get; private set; }
    
    public double RenderFrequency {
        get => _renderFrequency;
        set => _renderFrequency = Math.Max(1, value);
    }
    public double UpdateFrequency { get => RenderFrequency; set => RenderFrequency = value; }
    private double _renderFrequency = 300;
    
    public string Title {
        get => SDL.GetWindowTitleS(Window);
        set {
            SDL.SetWindowTitle(Window, value);
            _title = value;
        }
    }
    private string _title = "Yoru App";

    public Vector2 Size {
        get {
            int x, y;
            SDL.GetWindowSize(Window, &x, &y);
            return new(x, y);
        }
        set {
            SDL.SetWindowSize(Window, (int)value.X, (int)value.Y);
        }
    }

    public MouseButton GetMouseButton(byte button) {
        return button switch {
            1 => MouseButton.Left,
            2 => MouseButton.Middle,
            3 => MouseButton.Right,
            4 => MouseButton.Button4,
            5 => MouseButton.Button5,
            6 => MouseButton.Button6,
            7 => MouseButton.Button7,
            8 => MouseButton.Button8,
            _ => MouseButton.Left
        };
    }

    public Key GetKey(int key) {
        string enumSub = Enum.GetName(typeof(KeyCode), key).Substring(1);
        if (Enum.TryParse<Key>("D" + enumSub, out Key dRes))
            return dRes;

        if (Enum.TryParse<Key>(enumSub, out Key kRes))
            return kRes;
        
        KeyCode K = (KeyCode)key;

        return K switch {
            KeyCode.KReturn => Key.Enter,
            KeyCode.KBackquote => Key.GraveAccent,
            KeyCode.KLshift => Key.LeftShift,
            KeyCode.KRshift => Key.RightShift,
            KeyCode.KLctrl => Key.LeftControl,
            KeyCode.KRctrl => Key.RightControl,
            KeyCode.KLalt => Key.LeftAlt,
            KeyCode.KRalt => Key.RightAlt,
            KeyCode.KQuote => Key.Apostrophe,
            KeyCode.KLeftbracket => Key.LeftBracket,
            KeyCode.KRightbracket => Key.RightBracket,
            KeyCode.KEquals => Key.Equal,
            KeyCode.KCapslock => Key.CapsLock,
            KeyCode.KLgui => Key.LeftSuper,
            KeyCode.KRgui => Key.RightSuper,
            KeyCode.KPrintscreen => Key.PrintScreen,
            KeyCode.KScrolllock => Key.ScrollLock,
            KeyCode.KPageup => Key.PageUp,
            KeyCode.KPagedown => Key.PageDown,
            KeyCode.KNumlockclear => Key.NumLock,
            KeyCode.KApplication => Key.Menu,
            
            KeyCode.KKPDivide => Key.KeyPadDivide,
            KeyCode.KKPMultiply => Key.KeyPadMultiply,
            KeyCode.KKPMinus => Key.KeyPadSubtract,
            KeyCode.KKPPlus => Key.KeyPadAdd,
            KeyCode.KKPEnter => Key.KeyPadEnter,
            KeyCode.KKP1 => Key.KeyPad1,
            KeyCode.KKP2 => Key.KeyPad2,
            KeyCode.KKP3 => Key.KeyPad3,
            KeyCode.KKP4 => Key.KeyPad4,
            KeyCode.KKP5 => Key.KeyPad5,
            KeyCode.KKP6 => Key.KeyPad6,
            KeyCode.KKP7 => Key.KeyPad7,
            KeyCode.KKP8 => Key.KeyPad8,
            KeyCode.KKP9 => Key.KeyPad9,
            KeyCode.KKP0 => Key.KeyPad0,
            KeyCode.KKPPeriod => Key.KeyPadDecimal,

            _ => Key.Unknown
        };
    }

    public unsafe void Run() {
        if (SDL.Init(Sdl.InitVideo | Sdl.InitEvents) != 0)
            throw new Exception(SDL.GetErrorS());
        
        SDL.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Compatibility);
        SDL.GLSetAttribute(GLattr.ContextFlags, (int)GLcontextFlag.ForwardCompatibleFlag);

        Window = SDL.CreateWindow(Title, Sdl.WindowposCentered, Sdl.WindowposCentered, 800, 600, (uint)WindowFlags.Opengl);
        GLA = GL.GetApi(x => (IntPtr)SDL.GLGetProcAddress(x));

        SDL.SetWindowResizable(Window, SdlBool.True);
        SDL.SetWindowTitle(Window, _title);

        Context = SDL.GLCreateContext(Window);
        if (SDL.GLMakeCurrent(Window, Context) != 0)
            throw new Exception(SDL.GetErrorS());

        Renderer = new();
        App.Handler = this;
        App.Renderer = Renderer;
        App.Load();

        App.Resize(800, 600);

        while (Open) {
            Event evt = new();
            while (SDL.PollEvent(ref evt) != 0) {
                switch ((EventType)evt.Type) {
                    case EventType.Windowevent:
                        switch ((WindowEventID)evt.Window.Event) {
                            case WindowEventID.Close:
                                Close();
                                break;
                            case WindowEventID.Resized:
                                GLA.Viewport(0, 0, (uint)evt.Window.Data1, (uint)evt.Window.Data2);
                                App.Resize(evt.Window.Data1, evt.Window.Data2);
                                break;
                        }
                        break;

                    case EventType.Keydown:
                        if (evt.Key.Repeat != 0) break;
                        App.KeyDown(GetKey(evt.Key.Keysym.Sym));
                        break;

                    case EventType.Keyup:
                        App.KeyUp(GetKey(evt.Key.Keysym.Sym));
                        break;

                    case EventType.Mousebuttondown:
                        App.MouseDown(GetMouseButton(evt.Button.Button));
                        break;

                    case EventType.Mousebuttonup:
                        App.MouseUp(GetMouseButton(evt.Button.Button));
                        break;

                    case EventType.Mousemotion:
                        Vector2 pos = new(evt.Motion.X, evt.Motion.Y);
                        pos = Vector2.Clamp(pos, Vector2.Zero, Size);
                        App.MouseMove(pos);
                        break;
                }
            }

            if (SDL.GLMakeCurrent(Window, Context) != 0)
                throw new Exception(SDL.GetErrorS());

            App.Update();
            App.Render();

            SDL.GLSwapWindow(Window);

            SDL.PumpEvents();
            SDL.Delay((uint)(1000 / RenderFrequency));
        }

        SDL.GLDeleteContext(Context);
        SDL.DestroyWindow(Window);
        SDL.QuitSubSystem(Sdl.InitVideo | Sdl.InitEvents);
    }
}
