using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities;

public static class RenderingState
{
    private static MultisampleState multisampleState = new ();
    private static RasterizationState rasterizationState = new ();
    private static DepthState depthState = new ();
    private static StencilState stencilState = new ();
    private static BlendingState colorBlendAttachmentState = new ();

    public static MultisampleState MultisampleState
    {
        get => multisampleState;
        set
        {
            multisampleState = value;
            DefineState(value.Enable, EnableCap.Multisample);
            DefineState(value.SampleShadingEnable, EnableCap.SampleShading);
            DefineState(value.AlphaToCoverageEnable, EnableCap.SampleAlphaToCoverage);
            DefineState(value.AlphaToOneEnable, EnableCap.SampleAlphaToOne);
            GL.MinSampleShading(value.MinSampleShading);
            GL.SampleMask(value.SampleMaskNumber, value.SampleMaskValue);
        }
    }

    public static RasterizationState RasterizationState
    {
        get => rasterizationState;
        set
        {
            rasterizationState = value;

            DefineState(value.DepthClampEnable, EnableCap.DepthClamp);

            DefineState(value.CullFaceEnable, EnableCap.CullFace);
            GL.CullFace(value.CullMode);

            GL.PolygonMode(MaterialFace.FrontAndBack, value.PolygonMode);

            GL.FrontFace(value.FrontFaceDir);

            DefineState(value.DepthBiasEnable, EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(value.DepthBiasConstantFactor, value.DepthBiasSlopeFactor);

            GL.PointSize(value.PointSize);
            GL.LineWidth(value.LineWidth);

            DefineState(value.LineSmooth, EnableCap.LineSmooth);
        }
    }

    public static DepthState DepthState
    {
        get => depthState;
        set
        {
            depthState = value;

            DefineState(value.DepthTestEnable, EnableCap.DepthTest);
            GL.DepthMask(value.DepthWriteEnable);
            GL.DepthFunc(value.DepthCompareOp);
        }
    }

    public static StencilState StencilState
    {
        get => stencilState;
        set
        {
            stencilState = value;
            DefineState(value.Enable, EnableCap.StencilTest);
            DefineStencilFace(StencilFace.Front, value.Front);
            DefineStencilFace(StencilFace.Back, value.Back);
        }
    }

    public static BlendingState BlendingState
    {
        get => colorBlendAttachmentState;
        set
        {
            colorBlendAttachmentState = value;

            DefineState(value.Enable, EnableCap.Blend);
            GL.ColorMask(value.MaskComps.Red, value.MaskComps.Green, value.MaskComps.Blue, value.MaskComps.Alpha);
            GL.BlendEquationSeparate(value.ColorBlendMode, value.AlphaBlendMode);
            GL.BlendFuncSeparate(value.SrcColorBlendFactor, value.DstColorBlendFactor,
                value.SrcAlphaBlendFactor, value.DstAlphaBlendFactor);
        }
    }

    public static Color4 ClearColor
    {
        get
        {
            GL.GetFloat(GetPName.ColorClearValue, out Vector4 clearColor);
            return Unsafe.As<Vector4, Color4>(ref clearColor);
        }
        set => GL.ClearColor(value);
    }

    public static bool FramebufferSrgb
    {
        get => GL.IsEnabled(EnableCap.FramebufferSrgb);
        set => GL.Enable(EnableCap.FramebufferSrgb);
    }

    public static bool TextureCubeMapSeamless
    {
        get => GL.IsEnabled(EnableCap.TextureCubeMapSeamless);
        set => GL.Enable(EnableCap.TextureCubeMapSeamless);
    }

    private static bool DefineState(bool state, EnableCap enableCap)
    {
        if (state)
        {
            GL.Enable(enableCap);
        }
        else
        {
            GL.Disable(enableCap);
        }

        return state;
    }

    private static void DefineStencilFace(StencilFace face, StencilFaceState value)
    {
        GL.StencilOpSeparate(face, value.PassOp, value.FailOp, value.DepthFailOp);
        GL.StencilFuncSeparate(face, value.CompareOp, value.Reference, value.CompareMask);
        GL.StencilMaskSeparate(face, value.WriteMask);
    }
}
