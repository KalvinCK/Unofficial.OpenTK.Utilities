using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyRenderbufferObject : IReadOnlyBuffer
{
    public void Bind();

    public unsafe void GetParameter(RenderbufferParameterName parameterName, out int value);

    public unsafe void GetParameter(RenderbufferParameterName parameterName, int[] values);

    internal static int CreateBuffer()
    {
        GL.CreateRenderbuffers(1, out int buffer);
        return buffer;
    }
}
