#nullable disable

using System.Numerics;
using Silk.NET.SDL;
using Yoru.Input;

namespace Yoru.Platforms.SDL;

using GL = Silk.NET.OpenGL.GL;
using MouseButton = MouseButton;

public unsafe class SdlWindow : IApplicationHandler {
    private double _renderFrequency = 300;
    private string _title = "Yoru App";
    
    private bool _vsync = true;
    public Application App;
    public SdlRenderer Renderer;
    
    public bool Open { get; set; } = true;
    
    public Window* Window { get; private set; }
    public void* Context { get; private set; }
    public Sdl Sdl { get; } = Sdl.GetApi();
    public GL Gl { get; private set; }
    public void Close() => Open = false;
    
    public double RenderFrequency {
        get => _renderFrequency;
        set => _renderFrequency = Math.Max(1, value);
    }
    public double UpdateFrequency { get => RenderFrequency; set => RenderFrequency = value; }
    
    public string Title {
        get => Sdl.GetWindowTitleS(Window);
        set {
            Sdl.SetWindowTitle(Window, value);
            _title = value;
        }
    }
    
    public Vector2 Size {
        get {
            int x, y;
            Sdl.GetWindowSize(Window, &x, &y);
            return new(x, y);
        }
        set => Sdl.SetWindowSize(Window, (int)value.X, (int)value.Y);
    }
    
    public bool VSync {
        get => _vsync;
        set {
            _vsync = value;
            Sdl.GLSetSwapInterval(value ? 1 : 0);
        }
    }
    
    public void Run() {
        if (Sdl.Init(Sdl.InitVideo | Sdl.InitEvents) != 0)
            throw new(Sdl.GetErrorS());
        
        Sdl.GLSetAttribute(GLattr.ContextProfileMask, (int)GLprofile.Compatibility);
        
        if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            Sdl.GLSetAttribute(GLattr.ContextFlags, (int)GLcontextFlag.ForwardCompatibleFlag);
        
        Window = Sdl.CreateWindow(Title, Sdl.WindowposCentered, Sdl.WindowposCentered, 800, 600, (uint)WindowFlags.Opengl);
        Gl = GL.GetApi(x => (IntPtr)Sdl.GLGetProcAddress(x));
        
        Sdl.SetWindowResizable(Window, SdlBool.True);
        Sdl.SetWindowTitle(Window, _title);
        
        Context = Sdl.GLCreateContext(Window);
        if (Sdl.GLMakeCurrent(Window, Context) != 0)
            throw new(Sdl.GetErrorS());
        
        Renderer = new();
        App.Handler = this;
        App.Renderer = Renderer;
        App.Load();
        
        App.Resize(800, 600);
        VSync = _vsync;
        
        while (Open) {
            Event evt = new();
            while (Sdl.PollEvent(ref evt) != 0) {
                switch ((EventType)evt.Type) {
                    case EventType.Windowevent:
                        switch ((WindowEventID)evt.Window.Event) {
                            case WindowEventID.Close:
                                Close();
                                break;
                            case WindowEventID.Resized:
                                Gl.Viewport(0, 0, (uint)evt.Window.Data1, (uint)evt.Window.Data2);
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
            
            if (Sdl.GLMakeCurrent(Window, Context) != 0)
                throw new(Sdl.GetErrorS());
            
            App.Update();
            App.Render();
            
            Sdl.GLSwapWindow(Window);
            
            Sdl.PumpEvents();
            Sdl.Delay((uint)(1000 / RenderFrequency));
        }
        
        Sdl.GLDeleteContext(Context);
        Sdl.DestroyWindow(Window);
        Sdl.QuitSubSystem(Sdl.InitVideo | Sdl.InitEvents);
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
        var enumSub = Enum.GetName(typeof(KeyCode), key)?[1..];
        if (Enum.TryParse("D" + enumSub, out Key dRes))
            return dRes;
        
        if (Enum.TryParse(enumSub, out Key kRes))
            return kRes;
        
        return (KeyCode)key switch {
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
}
