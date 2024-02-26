global using OpenTK.Utilities.BufferObjects;
global using OpenTK.Utilities.Textures;
global using OpenTK.Utilities.Images;
global using OpenTK.Utilities.Objects;
global using ImGuiNET;

global using Image = OpenTK.Utilities.Images.Image;
using MathHelper = OpenTK.Mathematics.MathHelper;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Data.Common;
using OpenTK.Utilities;
using System.Text.RegularExpressions;
using System;
using Color4 = OpenTK.Mathematics.Color4;
using System.Text.Encodings.Web;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

public struct Transform
{
    public Vector3 Position;
    public Vector3 Scaling;
    public Quaternion Orientation;

    public readonly Matrix4x4 ModelMatrix
    {
        get
        {
            Matrix4x4 RotationMatrix = Matrix4x4.CreateFromQuaternion(Orientation);
            Matrix4x4 MatrixScale = Matrix4x4.CreateScale(Scaling);
            Matrix4x4 MatrixTranslation = Matrix4x4.CreateTranslation(Position);

            return MatrixScale * RotationMatrix * MatrixTranslation;
        }
    }

    public Transform(Vector3 p, Vector3 sc, Quaternion ortt)
    {
        Position = p;
        Scaling = sc;
        Orientation = ortt;
    }
    public override readonly string ToString()
    {
        return $"Position: {Position} Scaling: {Scaling} Orientation: {Orientation}";
    }
}


public struct ShitCamera()
{
    private float Radius = 5.0f;
    private float Pitch = 702f;
    private float Yaw = 30f;
    private Vector2 PreviousMouse = Vector2.Zero, SmoothedMouse = Vector2.Zero;

    public Vector3 Position;
    public Matrix4x4 View;
    public Matrix4x4 Projection;
    public void UpdateInputs(GameWindow window)
    {
        var MousePos = window.MouseState.Position;
        int width = window.Size.X;
        int height = window.Size.Y;

        if (window.IsKeyPressed(Keys.Escape)) window.Close();

        SmoothedMouse = Vector2.Lerp(SmoothedMouse, new Vector2(MousePos.X, MousePos.Y), 0.004f);
        Vector2 mouseDelta = SmoothedMouse - PreviousMouse;
        PreviousMouse = SmoothedMouse;

        Radius -= window.MouseState.ScrollDelta.Y * 0.06f;
        Yaw += mouseDelta.X * 0.8f;
        Pitch += mouseDelta.Y * 0.5f;

        Pitch = Math.Clamp(Pitch, -89f, 89f);


        Vector3 Orientation = Vector3.Transform(Vector3.UnitZ, Matrix4x4.CreateFromYawPitchRoll(MathHelper.DegreesToRadians(Yaw), MathHelper.DegreesToRadians(Pitch), 0));
        Vector3 pos = Vector3.Zero - Orientation * Radius;


        Position  = pos;
        View  = Matrix4x4.CreateLookAt(pos, Orientation, Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Vector3.Cross(Orientation, Vector3.UnitY)), Orientation)));
        Projection  = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(50.0f), width / (float)height, 0.01f, 100f);

    }
}
public unsafe sealed class App : IDisposable
{

    private GameWindow Window;
    private Size WinSize;

    private ImGuiBackend ImGuiBackend;
    private BufferConstant<Vector3> BufferConstant;
    private VertexArrayObject VertexArrayObject;
    private BufferImmutable<Data> BufferVertices;
    private BufferImmutable<uint> BufferElements;

    private Texture2D Texture;

    private PipelineObject Pipeline;
    private ShaderObject ShaderProgVert;
    private ShaderObject ShaderProgFrag;

    private ShaderObject ShaderObject;

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

        Context.Debug.Enable = true;

        ImGuiBackend = new ImGuiBackend(new Size(window.Size.X, window.Size.Y));

        Data[] Vertices =
        [
            new(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1.0f, 1.0f), new Vector4(1, 1, 1, 1)),
            new(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 0.0f), new Vector4(1, 1, 1, 1)),
            new(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 0.0f), new Vector4(1, 1, 1, 1)),
            new(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 1.0f), new Vector4(1, 1, 1, 1)),
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        // shaders programs
        // Note that it must be created in a separable way.
        ShaderProgVert = ShaderObject.CreateProgramSeparable(
            ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"));

        ShaderProgFrag = ShaderObject.CreateProgramSeparable(
            ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

        Pipeline = new PipelineObject();
        Pipeline.SetShader(ShaderProgVert, ShaderProgFrag);


        ShaderObject = new ShaderObject(
            ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"),
            ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

        var img = Image.FromFile("Resources/Goku Ultra Instinct 4K.jpg");
        Texture = new Texture2D(TextureFormat.Srgb8, img.Width, img.Height);
        Texture.Update(img.Width, img.Height, PixelFormat.Rgb, PixelType.UnsignedByte, img.Data);
        ShaderProgFrag.Uniform(0, Texture.BindlessHandler);

        BufferVertices = new BufferImmutable<Data>(Vertices, StorageUseFlag.ClientStorageBit);
        BufferElements = new BufferImmutable<uint>(indices, StorageUseFlag.ClientStorageBit);

        VertexArrayObject = new VertexArrayObject();
        VertexArrayObject.FixElementBuffer(BufferElements);
        VertexArrayObject.FixVertexBuffer(BufferVertices);

        VertexArrayObject.SetAttribFormat(0, 0, 3, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Pos"));
        VertexArrayObject.SetAttribFormat(0, 1, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("TexCoord"));
        VertexArrayObject.SetAttribFormat(0, 2, 4, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Color"));

        BufferConstant = new BufferConstant<Vector3>(new Vector3(1, 1, 1));
        BufferConstant.BindBufferBase(BufferRangeTarget.UniformBuffer, 0);

        Console.WriteLine($"Renderer: {Context.Device.Renderer}");
        Console.WriteLine($"Vendor: {Context.Device.Vendor}");
        Console.WriteLine($"Version: {Context.Device.Version}");
        Console.WriteLine($"Major Minor Version: {Context.Device.MajorMinorVersion}");
        Console.WriteLine($"Shading Language Version: {Context.Device.ShadingLanguageVersion}");
        Console.WriteLine($"Memory Total: {Context.Device.GpuMemory.MemoryTotal}");
        Console.WriteLine($"Memory Available: {Context.Device.GpuMemory.MemoryAvailable}");
        Console.WriteLine($"Memory Usage: {Context.Device.GpuMemory.MemoryUsage}");

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
            if(ImGui.Button("Clear cls"))
            {
                Console.Clear();
            }

            if (ImGui.ColorEdit3("Interp color", ref color1))
            {
                BufferConstant.Data = color1;
            }
            
        }
        ImGui.End();


        Pipeline.Bind();
        VertexArrayObject.Bind();
        Drawing.DrawElements(DrawElementsType.UnsignedInt, BufferElements.Count);
        Drawing.ResetDrawingContext();



        GL.Disable(EnableCap.DepthTest);
        ImGuiBackend.Render();
        GL.Enable(EnableCap.DepthTest);


    }
    public void Resize(Size size)
    {
        GL.ViewportIndexed(0, 0, 0, size.Width, size.Height);

        WinSize = size;
        ImGuiBackend.Size = size;
    }
    public void Dispose()
    {
        Texture?.Dispose();

        Pipeline?.Dispose();
        ShaderProgVert?.Dispose();
        ShaderProgFrag?.Dispose();

        ShaderObject?.Dispose();

        ImGuiBackend.Dispose();

        BufferConstant.Dispose();
        VertexArrayObject.Dispose();
        BufferVertices.Dispose();
        BufferElements.Dispose();
    }
}
