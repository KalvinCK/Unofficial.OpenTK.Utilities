﻿global using OpenTK.Utilities.BufferObjects;
global using OpenTK.Utilities.Textures;
global using OpenTK.Utilities.Images;
global using OpenTK.Utilities.Objects;
global using ImGuiNET;

global using Image = OpenTK.Utilities.Images.Image;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Data.Common;

namespace Test;

public static class Rand
{
    static readonly Random rand = new Random();

    public static float RandomFloat(float min, float max)
        => min + rand.NextSingle() * (max - min);
    public static Vector2 RandomVec2(float min, float max)
        => new Vector2(min) + new Vector2(rand.NextSingle(), rand.NextSingle()) * (max - min);
    public static Vector3 RandomVec3(float min, float max)
        => new Vector3(min) + new Vector3(rand.NextSingle(), rand.NextSingle(), rand.NextSingle()) * (max - min);
    public static Vector4 RandomVec4(float min, float max)
        => new Vector4(min) + new Vector4(rand.NextSingle(), rand.NextSingle(), rand.NextSingle(), rand.NextSingle()) * (max - min);
}

public struct Data
{
    public Vector3 Pos;
    public Vector2 TexCoord;
    public Vector4 Color;

    public Data(Vector3 p, Vector2 tx, Vector4 c)
    {
        Pos = p;
        TexCoord = tx;
        Color = c;
    }
    public override readonly string ToString()
    {
        return $"Pos: {Pos} TexCoord: {TexCoord} Color: {Color}";
    }
}

public unsafe sealed class App : IDisposable
{
    private GameWindow Window;
    private Size WinSize;

    private ImGuiBackend ImGuiBackend;
    private Shader Shader;
    private Texture2D Texture;
    private BufferConstant<Vector3> BufferConstant;
    private VertexArrayObject VertexArrayObject;
    private BufferImmutable<Data> BufferVertices;
    private BufferImmutable<uint> BufferElements;
    public unsafe App(GameWindow window)
    {
        Window = window;
        WinSize = new Size(window.Size.X, window.Size.Y);

        GL.Enable(EnableCap.FramebufferSrgb);
        //GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.TextureCubeMapSeamless);
        GL.Enable(EnableCap.LineSmooth);
        GL.ClearColor(0.01f, 0.01f, 0.01f, 1f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.DebugMessageCallback((DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) =>
        {
            if (id is 131169 || id is 131185 || id is 131218 || id is 131204)
                return;

            string printMsgWarning = $"ID: {id}\nSeverity: {severity}\nType: {type}\nMessage: {Marshal.PtrToStringAnsi(message, length)}\n";
            Console.WriteLine(printMsgWarning);

        }, IntPtr.Zero);

        ImGuiBackend = new ImGuiBackend(new Size(window.Size.X, window.Size.Y));

        Data[] Vertices =
        [
            new (new Vector3( 0.5f,  0.5f, 0.0f), new Vector2(1.0f, 1.0f), new Vector4(1, 1, 1, 1)),
            new (new Vector3( 0.5f, -0.5f, 0.0f), new Vector2(1.0f, 0.0f), new Vector4(1, 1, 1, 1)),
            new (new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 0.0f), new Vector4(1, 1, 1, 1)),
            new (new Vector3(-0.5f,  0.5f, 0.0f), new Vector2(0.0f, 1.0f), new Vector4(1, 1, 1, 1)),
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        Shader = new Shader(
            ShaderCompiled.CompileFromFileVertex("Resources/Vertex.vert"),
            ShaderCompiled.CompileFromFileFragment("Resources/Fragment.frag"));

        BufferConstant = new BufferConstant<Vector3>();
        BufferConstant.BindBufferBase(BufferRangeTarget.UniformBuffer, 0);

        string gokuImg = "Resources/Goku Ultra Instinct 4K.jpg";

        //var img = Image.FromFile(gokuImg);

        var img = Image.FromFile(gokuImg);

        Texture = new Texture2D(TextureFormat.Srgb8, img.Width, img.Height);

        Texture.Update(img.Width, img.Height, PixelFormat.Rgb, PixelType.UnsignedByte, img.Data);

        Shader.Uniform(0, Texture.BindlessHandler);

        BufferVertices = new BufferImmutable<Data>(Vertices);
        BufferElements = new BufferImmutable<uint>(indices);

        VertexArrayObject = new VertexArrayObject();
        VertexArrayObject.SetElementBuffer(BufferElements);
        VertexArrayObject.AddVertexBuffer(0, BufferVertices);

        VertexArrayObject.SetAttribFormat(0, 0, 3, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Pos"));
        VertexArrayObject.SetAttribFormat(0, 1, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("TexCoord"));
        VertexArrayObject.SetAttribFormat(0, 2, 4, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Color"));
    }

    private Vector3 color1 = Vector3.One;
    public void RenderFrame(FrameEventArgs args)
    {
        if (Window.IsKeyPressed(Keys.Escape))
        {
            Window.Close();
        }
        ImGuiBackend.Update(Window, (float)args.Time);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ImGui.Begin("Main");
        {
            if(ImGui.ColorEdit3("Interp color", ref color1))
            {
                BufferConstant.Data = color1;
            }

        }


        ImGui.End();

        Shader.Use();
        VertexArrayObject.Bind();

        GL.DrawElements(PrimitiveType.Triangles, BufferElements.Count, DrawElementsType.UnsignedInt, 0);

        if(ImGui.Button("SaveScree"))
        {
            using Texture2D screenTex = IFrameBufferObject.Default.ExtractTextureColor<Texture2D>(WinSize);
            TextureManager.SaveJpg(screenTex, "Resources", "ScreenShoot", 100);
        }


        GL.Disable(EnableCap.DepthTest);
        ImGuiBackend.Render();
        GL.Enable(EnableCap.DepthTest);
    }
    public void Resize(Size size)
    {
        WinSize = size;
        ImGuiBackend.Size = size;
    }
    public void Dispose()
    {
        ImGuiBackend.Dispose();

        BufferConstant.Dispose();
        Texture?.Dispose();
        Shader.Dispose();
        VertexArrayObject.Dispose();
        BufferVertices.Dispose();
        BufferElements.Dispose();
    }
}