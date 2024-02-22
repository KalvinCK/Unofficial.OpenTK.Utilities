using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities;

#pragma warning disable SA1201 // Elements should appear in the correct order
public class ContextGL
{
    static ContextGL()
    {
        var extensions = new HashSet<string>(GL.GetInteger(GetPName.NumExtensions));

        for (int i = 0; i < GL.GetInteger(GetPName.NumExtensions); i++)
        {
            extensions.Add(GL.GetString(StringNameIndexed.Extensions, i));
        }

        string extensionsFilePath = "GLExtensions.txt";
        if (!File.Exists(extensionsFilePath))
        {
            using StreamWriter writer = new StreamWriter(extensionsFilePath);
            foreach (string text in extensions)
            {
                writer.WriteLine(text);
            }
        }

        Extensions = extensions;
    }

    #region Debug
    private static readonly DebugProc DebugProcCallback = (DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) =>
    {
        if (id is 131169 || id is 131185 || id is 131218 || id is 131204)
        {
            return;
        }

        string printMsgWarning = $"Source: [{source.ToString().ToLower()}]\nID: {id}\nSeverity: [{severity.ToString().ToLower()}]\nLevel: [{type.ToString().ToLower()}]\nMessage: {Marshal.PtrToStringAnsi(message, length)}\n";
        Console.WriteLine(printMsgWarning);
    };

    private static bool enableDebug;

    public static bool EnableDebug
    {
        get => enableDebug;
        set
        {
            if (enableDebug == value)
            {
                return;
            }

            enableDebug = value;

            var state = GL.GetInteger(GetPName.ContextFlags) is (int)ContextFlagMask.ContextFlagDebugBit;

            if (state is false)
            {
                Debug.Print("Debug not created, check if debug has been activated in the window settings");
                return;
            }

            if (enableDebug)
            {
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
                GL.DebugMessageCallback(DebugProcCallback, IntPtr.Zero);
            }
            else
            {
                GL.Disable(EnableCap.DebugOutput);
                GL.Disable(EnableCap.DebugOutputSynchronous);
                GL.DebugMessageCallback(null, IntPtr.Zero);
            }
        }
    }
    #endregion

    private static Color4 clearColor;

    public static Color4 Color
    {
        get
        {
            unsafe
            {
                fixed (float* ptr = &clearColor.R)
                {
                    GL.GetFloat(GetPName.ColorClearValue, ptr);
                }
            }

            return clearColor;
        }
        set => GL.ClearColor(value);
    }

    public struct Cullface
    {
        public static bool Enable
        {
            get => GL.IsEnabled(EnableCap.CullFace);
            set => GL.Enable(EnableCap.CullFace);
        }

        public static CullFaceMode Mode
        {
            get => (CullFaceMode)GL.GetInteger(GetPName.CullFaceMode);
            set => GL.CullFace(value);
        }
    }

    public struct Line
    {
        public static bool Smooth
        {
            get => GL.IsEnabled(EnableCap.LineSmooth);
            set => GL.Enable(EnableCap.LineSmooth);
        }

        public static float Lenght
        {
            get => GL.GetFloat(GetPName.LineSmooth);
            set => GL.LineWidth(value);
        }
    }

    public struct DepthTest
    {
        private static bool mask;

        public static bool Enable
        {
            get => GL.IsEnabled(EnableCap.DepthTest);
            set => GL.Enable(EnableCap.DepthTest);
        }

        public static DepthFunction Func
        {
            get => (DepthFunction)GL.GetInteger(GetPName.DepthFunc);
            set => GL.DepthFunc(value);
        }

        public static bool Mask
        {
            get => mask;
            set => GL.DepthMask(mask = value);
        }
    }

    public struct Blending
    {
        private static bool maskRed = true;
        private static bool maskGreen = true;
        private static bool maskBlue = true;
        private static bool maskAlpha = true;

        public static bool ColorMaskRed
        {
            get => maskRed;
            set
            {
                maskRed = value;
                GL.ColorMask(maskRed, maskGreen, maskBlue, maskAlpha);
            }
        }

        public static bool ColorMaskGreen
        {
            get => maskGreen;
            set
            {
                maskGreen = value;
                GL.ColorMask(maskRed, maskGreen, maskBlue, maskAlpha);
            }
        }

        public static bool ColorMaskBlue
        {
            get => maskBlue;
            set
            {
                maskBlue = value;
                GL.ColorMask(maskRed, maskGreen, maskBlue, maskAlpha);
            }
        }

        public static bool ColorMaskAlpha
        {
            get => maskAlpha;
            set
            {
                maskAlpha = value;
                GL.ColorMask(maskRed, maskGreen, maskBlue, maskAlpha);
            }
        }

        public static bool Enable
        {
            get => GL.IsEnabled(EnableCap.Blend);
            set => GL.Enable(EnableCap.Blend);
        }

        public static Tuple<BlendingFactor, BlendingFactor> Func
        {
            get => new (
                (BlendingFactor)GL.GetInteger(GetPName.BlendSrc),
                (BlendingFactor)GL.GetInteger(GetPName.BlendDst));

            set => GL.BlendFunc(value.Item1, value.Item2);
        }

        public static BlendEquationMode Equation
        {
            get => (BlendEquationMode)GL.GetInteger(GetPName.BlendEquationRgb);
            set => GL.BlendEquation(value);
        }
    }

    #region Props

    public static string API { get; } = $"Driver: {GL.GetString(StringName.Version)}";

    public static string GPU { get; } = $"GPU: {GL.GetString(StringName.Renderer)}";

    public static string APIVersion { get; } = $"OpenGL Version: {Convert.ToDouble($"{GL.GetInteger(GetPName.MajorVersion)}{GL.GetInteger(GetPName.MinorVersion)}") / 10.0}";

    public static IReadOnlyCollection<string> Extensions { get; }

    public bool FramebufferSrgb
    {
        get => GL.IsEnabled(EnableCap.FramebufferSrgb);
        set => GL.Enable(EnableCap.FramebufferSrgb);
    }

    public bool Multisample
    {
        get => GL.IsEnabled(EnableCap.Multisample);
        set => GL.Enable(EnableCap.Multisample);
    }

    public bool TextureCubeMapSeamless
    {
        get => GL.IsEnabled(EnableCap.TextureCubeMapSeamless);
        set => GL.Enable(EnableCap.TextureCubeMapSeamless);
    }

    #endregion

    public static ErrorCode GetError()
    {
        return GL.GetError();
    }
}
#pragma warning restore SA1201 // Elements should appear in the correct order
