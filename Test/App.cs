global using OpenTK.Utilities.BufferObjects;
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
using OpenTK.Utilities;
using System.Text.RegularExpressions;
using System;

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

    private Pipeline Pipeline;
    private Shader ShaderProgVert;
    private Shader ShaderProgFrag;

    private Shader ShaderSample;

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

        ContextGL.EnableDebug = true;

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



        // shaders programs


        // Note that it must be created in a separable way.
        ShaderProgVert = Shader.CreateProgramSeparable(
            ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"));

        ShaderProgFrag = Shader.CreateProgramSeparable(
            ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

        Pipeline = new Pipeline();
        Pipeline.SetShader(ShaderProgVert, ShaderProgFrag);


        ShaderSample = Shader.CreateProgram(
            ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"),
            ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

        Pipeline = new Pipeline();
        Pipeline.SetShader(ShaderSample);
        Pipeline.SetShaderAllStages(ShaderSample);


        var img = Image.FromFile("Resources/Goku Ultra Instinct 4K.jpg");
        Texture = new Texture2D(TextureFormat.Srgb8, img.Width, img.Height);
        Texture.Update(img.Width, img.Height, PixelFormat.Rgb, PixelType.UnsignedByte, img.Data);
        ShaderProgFrag.Uniform(0, Texture.BindlessHandler);



        BufferVertices = new BufferImmutable<Data>(Vertices, StorageUseFlag.ClientStorageBit);
        BufferElements = new BufferImmutable<uint>(indices, StorageUseFlag.ClientStorageBit);

        VertexArrayObject = new VertexArrayObject();
        VertexArrayObject.SetElementBuffer(BufferElements);
        VertexArrayObject.AddVertexBuffer(0, BufferVertices);

        VertexArrayObject.SetAttribFormat(0, 0, 3, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Pos"));
        VertexArrayObject.SetAttribFormat(0, 1, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("TexCoord"));
        VertexArrayObject.SetAttribFormat(0, 2, 4, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Color"));

        BufferConstant = new BufferConstant<Vector3>();
        BufferConstant.BindBufferBase(BufferRangeTarget.UniformBuffer, 0);
        BufferConstant.Data = new Vector3(1, 1, 1);


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
            if (ImGui.ColorEdit3("Interp color", ref color1))
            {
                BufferConstant.Data = color1;
                //ShaderProg.Uniform(100, 100);
            }


        }
        ImGui.End();



        Pipeline.Bind();

        ShaderSample.Use();
        VertexArrayObject.Bind();
        GL.DrawElements(PrimitiveType.Triangles, BufferElements.Count, DrawElementsType.UnsignedInt, 0);

       


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
        Texture?.Dispose();

        Pipeline?.Dispose();
        ShaderProgVert?.Dispose();
        ShaderProgFrag?.Dispose();

        ImGuiBackend.Dispose();

        BufferConstant.Dispose();
        VertexArrayObject.Dispose();
        BufferVertices.Dispose();
        BufferElements.Dispose();
    }

    static void RemoverComentariosArquivo(string caminhoArquivo)
    {
        // Ler todo o conteúdo do arquivo para uma string
        string conteudo = File.ReadAllText(caminhoArquivo);

        // Remover comentários do conteúdo
        string conteudoSemComentarios = RemoverComentarios(conteudo);

        // Escrever o conteúdo sem comentários de volta para o mesmo arquivo
        File.WriteAllText(caminhoArquivo, conteudoSemComentarios);
    }

    static string RemoverComentarios(string texto)
    {
        // Remove comentários de linha (//) usando expressão regular
        texto = Regex.Replace(texto, @"//[^\r\n]*", "");

        // Remove comentários de bloco (/* */)
        texto = Regex.Replace(texto, @"/\*(.*?)\*/", "", RegexOptions.Singleline);

        // Remove linhas vazias
        texto = Regex.Replace(texto, @"^\s*$\n|\r", "", RegexOptions.Multiline);

        return texto;
    }
}
