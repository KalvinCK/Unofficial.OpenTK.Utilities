using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Objects;
using System.Linq.Expressions;
using System.Runtime;

namespace OpenTK.Utilities;

// font: https://github.com/JuanDiegoMontoya/Fwog/blob/main/include/Fwog/Pipeline____.h
public struct MultisampleState()
{
    public static readonly MultisampleState Default = new MultisampleState
    {
        SampleShadingEnable = false,
        MinSampleShading = 1,
        SampleMask = 0xFFFFFFFF,
        AlphaToCoverageEnable = false,
        AlphaToOneEnable = false,
    };

    public bool SampleShadingEnable;        // glEnable(GL_SAMPLE_SHADING)
    public float MinSampleShading;          // glMinSampleShading
    public uint SampleMask;                 // glSampleMaski
    public bool AlphaToCoverageEnable;      // glEnable(GL_SAMPLE_ALPHA_TO_COVERAGE)
    public bool AlphaToOneEnable;           // glEnable(GL_SAMPLE_ALPHA_TO_ONE)
}

public struct RasterizationState()
{
    public static readonly RasterizationState Default = new RasterizationState
    {
        DepthClampEnable = false,
        PolygonMode = PolygonMode.Fill,
        CullMode = CullFaceMode.Back,
        FrontFace = FrontFaceDirection.Ccw,
        DepthBiasEnable = false,
        DepthBiasConstantFactor = 0,
        DepthBiasSlopeFactor = 0,
        PointSize = 1,
        LineWidth = 1,
    };

    public bool DepthClampEnable;
    public PolygonMode PolygonMode;
    public CullFaceMode CullMode;
    public FrontFaceDirection FrontFace;
    public bool DepthBiasEnable;
    public float DepthBiasConstantFactor;
    public float DepthBiasSlopeFactor;
    public float PointSize; // glPointSize
    public float LineWidth; // glLineWidth
}

public struct DepthState()
{
    public static readonly DepthState Default = new DepthState
    {
        DepthTestEnable = false,
        DepthWriteEnable = false,
        DepthCompareOp = DepthFunction.Less,
    };

    public bool DepthTestEnable = false;           // gl{Enable, Disable}(GL_DEPTH_TEST)
    public bool DepthWriteEnable = false;           // glDepthMask(depthWriteEnable)
    public DepthFunction DepthCompareOp = DepthFunction.Less; // glDepthFunc
}

public struct StencilOpState()
{
    public StencilOp PassOp = StencilOp.Keep;   // glStencilOp (dppass)
    public StencilOp FailOp = StencilOp.Keep;   // glStencilOp (sfail)
    public StencilOp DepthFailOp = StencilOp.Keep;   // glStencilOp (dpfail)
    public StencilFunction CompareOp = StencilFunction.Always; // glStencilFunc (func)
    public uint CompareMask = 0;                 // glStencilFunc (mask)
    public uint WriteMask = 0;                 // glStencilMask
    public uint Reference = 0;                 // glStencilFunc (ref)
}

public struct StencilState()
{
    public bool StencilTestEnable = false;
    public StencilOpState Front;
    public StencilOpState Back;
}

public struct ColorComponent(bool red, bool green, bool blue, bool alpha)
{
    public static readonly ColorComponent AllComponents = new (true, true, true, true);
    public static readonly ColorComponent NoneComponents = new (false, false, false, false);
    public static readonly ColorComponent AlphaComponent = new (false, false, false, true);
    public static readonly ColorComponent RedComponent = new (true, false, false, false);
    public static readonly ColorComponent GreenComponent = new (false, true, false, false);
    public static readonly ColorComponent BlueComponent = new (false, false, true, false);

    public bool Red = red;
    public bool Green = green;
    public bool Blue = blue;
    public bool Alpha = alpha;
}

// glBlendFuncSeparatei + glBlendEquationSeparatei
public struct ColorBlendAttachmentState()
{
    public bool BlendEnable = false;                                           // if false, blend factor = one?
    public BlendingFactor SrcColorBlendFactor = BlendingFactor.One;              // srcRGB
    public BlendingFactor DstColorBlendFactor = BlendingFactor.Zero;             // dstRGB
    public BlendEquationMode ColorBlendOp = BlendEquationMode.FuncAdd;         // modeRGB
    public BlendingFactor SrcAlphaBlendFactor = BlendingFactor.One;              // srcAlpha
    public BlendingFactor DstAlphaBlendFactor = BlendingFactor.Zero;             // dstAlpha
    public BlendEquationMode AlphaBlendOp = BlendEquationMode.FuncAdd;         // modeAlpha
    public ColorComponent ColorMask = ColorComponent.AllComponents;             // glColorMaski
}

public class Pipeline : IDisposable
{
    public Pipeline()
    {
        GL.CreateProgramPipelines(1, out int buffer);
        this.BufferID = buffer;
    }

    public int BufferID { get; private set; }

    public string InfoLog
    {
        get
        {
            GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.ValidateStatus, out int param);

            string infoLog = string.Empty;
            if (param > 0)
            {
                GL.GetProgramPipelineInfoLog(this.BufferID, 1028, out int _, out infoLog);
            }

            return string.IsNullOrEmpty(infoLog) ? "OK!" : infoLog;
        }
    }

    public static void ClearContext()
    {
        GL.BindProgramPipeline(0);
    }

    public void Validate()
    {
        GL.ValidateProgramPipeline(this.BufferID);
    }

    public void Bind()
    {
        GL.BindProgramPipeline(this.BufferID);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public BufferRef GetID(ShaderType ShaderType)
    {
        int shaderID = 0;

        switch (ShaderType)
        {
            case ShaderType.VertexShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.VertexShader, out shaderID);
                break;
            case ShaderType.FragmentShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.FragmentShader, out shaderID);
                break;
            case ShaderType.GeometryShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.GeometryShader, out shaderID);
                break;
            case ShaderType.TessEvaluationShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.TessEvaluationShader, out shaderID);
                break;
            case ShaderType.TessControlShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.TessControlShader, out shaderID);
                break;
            case ShaderType.ComputeShader:
                GL.GetProgramPipeline(this.BufferID, ProgramPipelineParameter.ComputeShader, out shaderID);
                break;
        }

        return new BufferRef(shaderID);
    }

    public void SetShaderAllStages<TShader>(TShader shader)
        where TShader : IShader
    {
        if (shader.Separable is false)
        {
            throw new ShaderSeparableExeption();
        }

        GL.UseProgramStages(this.BufferID, ProgramStageMask.AllShaderBits, shader.BufferID);
    }

    public void SetShader(params IShader[] shaders)
    {
        foreach (var shader in shaders)
        {
            if (shader.Separable is false)
            {
                throw new ShaderSeparableExeption();
            }

            ProgramStageMask stageMask = 0;
            foreach (var type in shader.Composion)
            {
                stageMask |= ToStageMask(type);
            }

            GL.UseProgramStages(this.BufferID, stageMask, shader.BufferID);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteProgramPipeline(this.BufferID);
            this.BufferID = 0;
        }
    }

    private static ProgramStageMask ToStageMask(ShaderType shaderType)
    {
        return shaderType switch
        {
            ShaderType.VertexShader => ProgramStageMask.VertexShaderBit,
            ShaderType.FragmentShader => ProgramStageMask.FragmentShaderBit,
            ShaderType.GeometryShader => ProgramStageMask.GeometryShaderBit,
            ShaderType.TessEvaluationShader => ProgramStageMask.TessEvaluationShaderBit,
            ShaderType.TessControlShader => ProgramStageMask.TessControlShaderBit,
            ShaderType.ComputeShader => ProgramStageMask.ComputeShaderBit,
            _ => throw new InvalidEnumException<ShaderType>()
        };
    }
}

#pragma warning disable SA1202 // Elements should be ordered by access
public class Pipeline____
{
    private IBuffer shader;
    private IBuffer vao = IVertexArrayObject.Empty;
    private IBuffer framebf = IFrameBufferObject.Default;

    public void SetShader<T>(T shader)
        where T : IShader
    {
        if (this.shader.BufferID != shader.BufferID)
        {
            this.shader = shader;
            shader.Use();
        }
    }

    public void SetVertexArray<T>(T vertexArray)
        where T : IVertexArrayObject
    {
        if (this.vao.BufferID != vertexArray.BufferID)
        {
            this.vao = vertexArray;
            vertexArray.Bind();
        }
    }

    public void SetFrameBuffer<T>(T framebuffer)
        where T : IFrameBufferObject
    {
        if (this.framebf.BufferID != framebuffer.BufferID)
        {
            this.framebf = framebuffer;
            framebuffer.Bind();
        }
    }

    private void BufferEquals<T>(IBuffer inBuffer)
        where T : IBuffer
    {
        if (typeof(IShader) == typeof(IShader))
        {
            if (this.shader != inBuffer)
            {
                this.shader = inBuffer;
            }
        }
        else if (typeof(IShader) == typeof(IShader))
        {
            if (this.shader != inBuffer)
            {
                this.shader = inBuffer;
            }
        }
        else if (typeof(IShader) == typeof(IShader))
        {
            if (this.shader != inBuffer)
            {
                this.shader = inBuffer;
            }
        }
    }

    public static void Arrays<T>(
        in T vertexArray,
        in Shader shader,
        PrimitiveType PrimitiveType,
        int count,
        int first = 0)
        where T : IVertexArrayObject
    {
        Binds(vertexArray, shader);
        GL.DrawArrays(PrimitiveType, first, count);
        ClearContexts();
    }

    public static void ArraysIntanced<T>(
        in T vertexArray,
        in Shader shader,
        PrimitiveType PrimitiveType,
        int count,
        int instanceCount,
        int first = 0)
        where T : IVertexArrayObject

    {
        Binds(vertexArray, shader);
        GL.DrawArraysInstanced(PrimitiveType, first, count, instanceCount);
        ClearContexts();
    }

    public static void Elements<T>(
        in T vertexArray,
        in Shader shader,
        PrimitiveType PrimitiveType,
        DrawElementsType DrawElementsType,
        int count,
        int indices = 0)
        where T : IVertexArrayObject
    {
        Binds(vertexArray, shader);
        GL.DrawElements(PrimitiveType, count, DrawElementsType, indices);
        ClearContexts();
    }

    public static void ElementsInstanced<T>(
        in T vertexArray,
        in Shader shader,
        PrimitiveType PrimitiveType,
        DrawElementsType DrawElementsType,
        int count,
        int instanceCount,
        int indices = 0)
        where T : IVertexArrayObject
    {
        Binds(vertexArray, shader);
        GL.DrawElementsInstanced(PrimitiveType, count, DrawElementsType, indices, instanceCount);
        ClearContexts();
    }

    private static void Binds<T>(in T vertexArray, in Shader shader)
        where T : IVertexArrayObject
    {
        shader.Use();
        vertexArray.Bind();
    }

    private static void ClearContexts()
    {
        IShader.ClearContext();
        IVertexArrayObject.ClearContext();
    }
}
#pragma warning restore SA1202 // Elements should be ordered by access
