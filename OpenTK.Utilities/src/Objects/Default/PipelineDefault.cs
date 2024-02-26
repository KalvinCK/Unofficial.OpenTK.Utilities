using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public readonly struct PipelineDefault : IReadOnlyPipelineObject
{
    public int BufferID { get; }

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

    public void Validate()
    {
        GL.ValidateProgramPipeline(this.BufferID);
    }

    public void Bind()
    {
        GL.BindProgramPipeline(this.BufferID);
    }
}
