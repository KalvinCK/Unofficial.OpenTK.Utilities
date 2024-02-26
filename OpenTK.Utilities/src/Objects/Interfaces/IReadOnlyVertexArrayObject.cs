using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyVertexArrayObject : IReadOnlyBuffer
{
    public void Bind();

    internal static int CreateBuffer()
    {
        GL.CreateVertexArrays(1, out int buffer);
        return buffer;
    }
}
