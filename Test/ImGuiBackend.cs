using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Color4 = OpenTK.Mathematics.Color4;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Objects;
using OpenTK.Utilities.BufferObjects;
using OpenTK.Utilities.Textures;

namespace Test;

public class ImGuiBackend : IDisposable
{
    public bool IsIgnoreMouseInput { get; set; } = false;

    private readonly Shader ShaderProg;
    private readonly Texture2D FontTexture;
    private readonly VertexArrayObject Vao;
    private readonly BufferUnstructured VertBuffer;
    private readonly BufferUnstructured ElemBuffer;

    private readonly int VertStride = Unsafe.SizeOf<ImDrawVert>();
    private readonly int ElemStride = Unsafe.SizeOf<ushort>();

    private bool frameBegun;
    float _Gamma;
    public float Gamma
    {
        get => _Gamma;
        set
        {
            _Gamma = value;
            ShaderProg.Uniform(1, Gamma);
        }
    }

    private Vector2 scaleFactor = Vector2.One;
    private Size _Size;
    public Size Size
    {
        get => _Size;
        set
        {
            _Size = value;

            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new Vector2(_Size.Width / scaleFactor.X, _Size.Height / scaleFactor.Y);
            io.DisplayFramebufferScale = scaleFactor;

            var projection = Matrix4x4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);
            ShaderProg.Uniform(0, projection);
        }
    }

    public ImGuiBackend(Size windowSize)
    {

        IntPtr context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;

        // create devices resources
        #region Shaders Codes
        string vertexSource = @"#version 460 core

            layout(location = 0) in vec2 Position;
            layout(location = 1) in vec2 TexCoord;
            layout(location = 2) in vec4 Color;

            layout(location = 0) uniform mat4 projection;

            out InOutVars
            {
                vec4 Color;
                vec2 TexCoord;
            } outData;

            void main()
            {
                outData.Color = Color;
                outData.TexCoord = TexCoord;
                gl_Position = projection * vec4(Position, 0.0, 1.0);
            }";

        string fragmentSource = @"#version 460 core

            layout(location = 0) out vec4 FragColor;

            layout(binding = 0) uniform sampler2D SamplerFontTexture;
            layout(location = 1) uniform float Gamma;

            in InOutVars
            {
                vec4 Color;
                vec2 TexCoord;
            } inData;

            void main()
            {
                vec4 color = inData.Color * texture(SamplerFontTexture, inData.TexCoord);
                color.rgb = pow(color.rgb, vec3(Gamma));
                FragColor = color;
            }";
        #endregion

        ShaderProg = Shader.CreateProgram(
            ShaderSource.FromText(ShaderType.VertexShader, vertexSource),
            ShaderSource.FromText(ShaderType.FragmentShader, fragmentSource));


        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

        FontTexture = new Texture2D(TextureFormat.Rgba8, width, height);
        FontTexture.Filtering = TextureFiltering.Nearest;
        FontTexture.Wrapping = Texture2DWrapping.ClampToEdge;
        FontTexture.Update(width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

        io.Fonts.SetTexID(FontTexture.BufferID);
        io.Fonts.ClearTexData();

        ElemBuffer = new BufferUnstructured();
        ElemBuffer.ReserveImmutableMemory(10000, IntPtr.Zero);
        VertBuffer = new BufferUnstructured();
        ElemBuffer.ReserveImmutableMemory(2000, IntPtr.Zero);

        Vao = new VertexArrayObject();
        Vao.SetElementBuffer(ElemBuffer);
        Vao.AddVertexBuffer(0, VertBuffer, VertStride);
        Vao.SetAttribFormat(0, 0, 2, VertexAttribType.Float, 0 * sizeof(float));
        Vao.SetAttribFormat(0, 1, 2, VertexAttribType.Float, 2 * sizeof(float));
        Vao.SetAttribFormat(0, 2, 4, VertexAttribType.UnsignedByte, 4 * sizeof(float), true);

        Gamma = 2.0f;
        Size = windowSize;
        SetKeyMappings();
        SetStyle();
        SetPerFrameImGuiData(1.0f / 60.0f);

        ImGui.NewFrame();
        frameBegun = true;
    }
    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
    }

    public void Update(NativeWindow nwd, float elapsedTime)
    {
        if (frameBegun)
            ImGui.Render();

        SetPerFrameImGuiData(elapsedTime);
        UpdateImGuiInput(nwd);

        frameBegun = true;
        ImGui.NewFrame();
    }
    private static void SetPerFrameImGuiData(float dT)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DeltaTime = dT;
    }

    
    public void PressChar(char keyChar)
    {
        pressedChars.Add(keyChar);
    }
    public void MouseScroll(float offsetX, float offsetY)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        io.MouseWheel = offsetY;
        io.MouseWheelH = offsetX;
    }
    private readonly List<char> pressedChars = new List<char>();
    private void UpdateImGuiInput(NativeWindow nwd)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        MouseState MouseState = nwd.MouseState;
        KeyboardState KeyboardState = nwd.KeyboardState;

        io.MouseDown[0] = MouseState[MouseButton.Left];
        io.MouseDown[1] = MouseState[MouseButton.Right];
        io.MouseDown[2] = MouseState[MouseButton.Middle];

        if (IsIgnoreMouseInput)
            io.MousePos = new Vector2(-1.0f);
        else
            io.MousePos = new Vector2(nwd.MouseState.Position.X, nwd.MouseState.Position.Y);

        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
            if (key == Keys.Unknown)
            {
                continue;
            }
            io.KeysDown[(int)key] = KeyboardState.IsKeyDown(key) || KeyboardState.IsKeyPressed(key);
        }
        foreach (var c in pressedChars)
        {
            io.AddInputCharacter(c);
        }
        pressedChars.Clear();

        io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
        io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
        io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
        io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);
    }

    public void Render()
    {
        if (frameBegun)
        {
            frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }
    }
    private void RenderImDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0)
            return;

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[i];
            int vertexSize = cmdList.VtxBuffer.Size * VertStride;
            if (vertexSize > VertBuffer.MemoryBytesSize)
            {
                int newSize = (int)Math.Max(VertBuffer.MemoryBytesSize * 1.5f, vertexSize);
                VertBuffer.ReserveImmutableMemory(newSize, IntPtr.Zero);
            }

            int indexSize = cmdList.IdxBuffer.Size * ElemStride;
            if (indexSize > ElemBuffer.MemoryBytesSize)
            {
                int newSize = (int)Math.Max(ElemBuffer.MemoryBytesSize * 1.5f, indexSize);
                ElemBuffer.ReserveImmutableMemory(newSize, IntPtr.Zero);

            }
        }

        
        ShaderProg.Use();
        Vao.Bind();

        ImGuiIOPtr io = ImGui.GetIO();
        drawData.ScaleClipRects(io.DisplayFramebufferScale);
        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmd_list = drawData.CmdLists[i];

            VertBuffer.UpdateMemory(0, cmd_list.VtxBuffer.Size * VertStride, cmd_list.VtxBuffer.Data);
            ElemBuffer.UpdateMemory(0, cmd_list.IdxBuffer.Size * ElemStride, cmd_list.IdxBuffer.Data);

            int idx_offset = 0;

            for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
            {
                ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                if (pcmd.UserCallback != IntPtr.Zero)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    GL.BindTextureUnit(0, (int)pcmd.TextureId);

                    Vector4 clip = pcmd.ClipRect;
                    GL.Scissor((int)clip.X, Size.Height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                    if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), 0);
                    else
                        GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                }

                idx_offset += (int)pcmd.ElemCount;
            }
        }

        IShader.ClearContext();

    }

    #region Style
    private static unsafe void SetStyle()
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        RangeAccessor<Color4> colors = new RangeAccessor<Color4>(style.Colors.Data, style.Colors.Count);

        colors[(int)ImGuiCol.Text] = new Color4(1.000f, 1.000f, 1.000f, 1.000f);
        colors[(int)ImGuiCol.TextDisabled] = new Color4(0.500f, 0.500f, 0.500f, 1.000f);
        colors[(int)ImGuiCol.WindowBg] = new Color4(0.180f, 0.180f, 0.180f, 1.000f);
        colors[(int)ImGuiCol.ChildBg] = new Color4(0.280f, 0.280f, 0.280f, 0.000f);
        colors[(int)ImGuiCol.PopupBg] = new Color4(0.313f, 0.313f, 0.313f, 1.000f);
        colors[(int)ImGuiCol.Border] = Color4.BlueViolet;
        colors[(int)ImGuiCol.BorderShadow] = new Color4(0.000f, 0.000f, 0.000f, 0.000f);
        colors[(int)ImGuiCol.FrameBg] = new Color4(0.160f, 0.160f, 0.160f, 1.000f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Color4(0.200f, 0.200f, 0.200f, 1.000f);
        colors[(int)ImGuiCol.FrameBgActive] = new Color4(0.280f, 0.280f, 0.280f, 1.000f);
        colors[(int)ImGuiCol.TitleBg] = new Color4(0.148f, 0.148f, 0.148f, 1.000f);
        colors[(int)ImGuiCol.TitleBgActive] = Color4.BlueViolet;
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Color4(0.148f, 0.148f, 0.148f, 1.000f);
        colors[(int)ImGuiCol.MenuBarBg] = new Color4(0.195f, 0.195f, 0.195f, 1.000f);
        colors[(int)ImGuiCol.ScrollbarBg] = new Color4(0.160f, 0.160f, 0.160f, 1.000f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Color4(0.277f, 0.277f, 0.277f, 1.000f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Color4(0.300f, 0.300f, 0.300f, 1.000f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.CheckMark] = new Color4(1.000f, 1.000f, 1.000f, 1.000f);
        colors[(int)ImGuiCol.SliderGrab] = new Color4(0.391f, 0.391f, 0.391f, 1.000f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.Button] = new Color4(1.000f, 1.000f, 1.000f, 0.000f);
        colors[(int)ImGuiCol.ButtonHovered] = new Color4(1.000f, 1.000f, 1.000f, 0.156f);
        colors[(int)ImGuiCol.ButtonActive] = new Color4(1.000f, 1.000f, 1.000f, 0.391f);
        colors[(int)ImGuiCol.Header] = new Color4(0.313f, 0.313f, 0.313f, 1.000f);
        colors[(int)ImGuiCol.HeaderHovered] = new Color4(0.469f, 0.469f, 0.469f, 1.000f);
        colors[(int)ImGuiCol.HeaderActive] = new Color4(0.469f, 0.469f, 0.469f, 1.000f);
        colors[(int)ImGuiCol.Separator] = Color4.WhiteSmoke;
        colors[(int)ImGuiCol.SeparatorHovered] = new Color4(0.391f, 0.391f, 0.391f, 1.000f);
        colors[(int)ImGuiCol.SeparatorActive] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.ResizeGrip] = new Color4(1.000f, 1.000f, 1.000f, 0.250f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Color4(1.000f, 1.000f, 1.000f, 0.670f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.Tab] = new Color4(0.098f, 0.098f, 0.098f, 1.000f);
        colors[(int)ImGuiCol.TabHovered] = new Color4(0.352f, 0.352f, 0.352f, 1.000f);
        colors[(int)ImGuiCol.TabActive] = new Color4(0.195f, 0.195f, 0.195f, 1.000f);
        colors[(int)ImGuiCol.TabUnfocused] = new Color4(0.098f, 0.098f, 0.098f, 1.000f);
        colors[(int)ImGuiCol.TabUnfocusedActive] = new Color4(0.195f, 0.195f, 0.195f, 1.000f);
        colors[(int)ImGuiCol.DockingPreview] = new Color4(1.000f, 0.391f, 0.000f, 0.781f);
        colors[(int)ImGuiCol.DockingEmptyBg] = new Color4(0.180f, 0.180f, 0.180f, 1.000f);
        colors[(int)ImGuiCol.PlotLines] = new Color4(0.469f, 0.469f, 0.469f, 1.000f);
        colors[(int)ImGuiCol.PlotLinesHovered] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.PlotHistogram] = new Color4(0.586f, 0.586f, 0.586f, 1.000f);
        colors[(int)ImGuiCol.PlotHistogramHovered] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.TextSelectedBg] = new Color4(1.000f, 1.000f, 1.000f, 0.156f);
        colors[(int)ImGuiCol.DragDropTarget] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.NavHighlight] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.NavWindowingHighlight] = new Color4(1.000f, 0.391f, 0.000f, 1.000f);
        colors[(int)ImGuiCol.NavWindowingDimBg] = new Color4(0.000f, 0.000f, 0.000f, 0.586f);
        colors[(int)ImGuiCol.ModalWindowDimBg] = new Color4(0.000f, 0.000f, 0.000f, 0.586f);

        style.ChildRounding = 4.0f;
        style.FrameBorderSize = 1.0f;
        style.FrameRounding = 2.0f;
        style.GrabMinSize = 7.0f;
        style.PopupRounding = 2.0f;
        style.ScrollbarRounding = 12.0f;
        style.ScrollbarSize = 13.0f;
        style.TabBorderSize = 1.0f;
        style.TabRounding = 0.0f;
        style.WindowRounding = 4.0f;
    }
    #endregion

    public void Dispose()
    {
        FontTexture.Dispose();
        ShaderProg.Dispose();
        Vao.Dispose();
        VertBuffer.Dispose();
        ElemBuffer.Dispose();
    }
}
