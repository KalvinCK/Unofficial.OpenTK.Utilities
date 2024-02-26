using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class PipelineObject : IReadOnlyPipelineObject, IDisposable
{
    private readonly Dictionary<ProgramStageMask, IReadOnlyShaderObject> pipelineStorage;

    public PipelineObject()
    {
        var defult = Context.Default.DefaultShader;
        this.pipelineStorage = new Dictionary<ProgramStageMask, IReadOnlyShaderObject>
        {
            { ProgramStageMask.VertexShaderBit, defult },
            { ProgramStageMask.FragmentShaderBit, defult },
            { ProgramStageMask.GeometryShaderBit, defult },
            { ProgramStageMask.TessEvaluationShaderBit, defult },
            { ProgramStageMask.TessControlShaderBit, defult },
            { ProgramStageMask.ComputeShaderBit, defult },
        };

        this.BufferID = IReadOnlyPipelineObject.CreateBuffer();
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
                GL.GetProgramPipelineInfoLog(this.BufferID, 512, out int _, out infoLog);
            }

            return string.IsNullOrEmpty(infoLog) ? "OK!" : infoLog;
        }
    }

    public IReadOnlyShaderObject this[ProgramStageMask stage]
    {
        get => this.pipelineStorage[stage];
        set
        {
            var shader = value;

            ValidateShader(shader);

            foreach (var type in shader.Composion)
            {
                this.pipelineStorage[ToStageMask(type)] = shader;
            }

            GL.UseProgramStages(this.BufferID, ProgramStageMask.AllShaderBits, shader.BufferID);
        }
    }

    public void SetShaderAllStages(params IReadOnlyShaderObject[] shaders)
    {
        foreach (var shader in shaders)
        {
            ValidateShader(shader);

            foreach (var type in shader.Composion)
            {
                this.pipelineStorage[ToStageMask(type)] = shader;
            }

            GL.UseProgramStages(this.BufferID, ProgramStageMask.AllShaderBits, shader.BufferID);
        }
    }

    public void SetShader(params IReadOnlyShaderObject[] shaders)
    {
        foreach (var shader in shaders)
        {
            ValidateShader(shader);

            ProgramStageMask stageMask = 0;
            foreach (var type in shader.Composion)
            {
                this.pipelineStorage[ToStageMask(type)] = shader;
                stageMask |= ToStageMask(type);
            }

            GL.UseProgramStages(this.BufferID, stageMask, shader.BufferID);
        }
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

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteProgramPipeline(this.BufferID);
            this.BufferID = 0;
            this.pipelineStorage.Clear();
        }
    }

    private static ProgramStageMask ToStageMask(ShaderType type)
    {
        return type switch
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

    private static ShaderType ToShaderType(ProgramStageMask stage)
    {
        return stage switch
        {
             ProgramStageMask.VertexShaderBit => ShaderType.VertexShader,
             ProgramStageMask.FragmentShaderBit => ShaderType.FragmentShader,
             ProgramStageMask.GeometryShaderBit => ShaderType.GeometryShader,
             ProgramStageMask.TessEvaluationShaderBit => ShaderType.TessEvaluationShader,
             ProgramStageMask.TessControlShaderBit => ShaderType.TessControlShader,
             ProgramStageMask.ComputeShaderBit => ShaderType.ComputeShader,
            _ => throw new InvalidEnumException<ProgramStageMask>()
        };
    }

    private static void ValidateShader(IReadOnlyShaderObject shader)
    {
        if (shader.Separable is false)
        {
            throw new ShaderSeparableExeption();
        }
    }
}
