
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Drawing;
using Test;

namespace LocalTest;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

class Window : GameWindow
{
    static void Main(string[] args)
    {
        GameWindowSettings gwSettings = GameWindowSettings.Default;

        NativeWindowSettings nwSettings = new NativeWindowSettings()
        {
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6),
            AutoLoadBindings = true,
            Flags = ContextFlags.Debug | ContextFlags.ForwardCompatible,
            IsEventDriven = false,
            Profile = ContextProfile.Core,
            Size = (1000, 1000),
            Location = (0, 0),
            StartFocused = true,
            StartVisible = true,
            Title = $"Tests",
            WindowBorder = WindowBorder.Resizable,
            WindowState = WindowState.Normal,

        };

        using Window window = new Window(gwSettings, nwSettings);
        window.Run();
    }

    public Window(GameWindowSettings gwSettings, NativeWindowSettings nwSettings) : base(gwSettings, nwSettings)
    {
    }

    private App? App;
    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
        App = new App(this);



    }

    protected override void OnUnload()
    {
        base.OnUnload();
        App?.Dispose();
    }
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        App?.RenderFrame(args);
        SwapBuffers();
    }
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        App?.Resize(new Size(e.Width, e.Height));
        GL.Viewport(new Point(0, 0), new Size(e.Width, e.Height));
    }
}
