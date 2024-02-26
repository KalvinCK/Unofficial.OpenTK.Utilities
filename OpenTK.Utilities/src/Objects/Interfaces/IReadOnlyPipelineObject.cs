using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyPipelineObject : IReadOnlyBuffer
{
    public string InfoLog { get; }

    public void Validate();

    public void Bind();

    static int CreateBuffer()
    {
        GL.CreateProgramPipelines(1, out int buffer);
        return buffer;
    }
}
