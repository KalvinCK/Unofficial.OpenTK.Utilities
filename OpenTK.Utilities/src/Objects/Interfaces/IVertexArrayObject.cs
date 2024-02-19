using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IVertexArrayObject : IBuffer
{
    public static readonly IVertexArrayObject Empty = new VertexArrayObject();

    public static int BufferBindedInContext { get; internal set; }

    public void Bind();

    internal static int CreateBuffer()
    {
        GL.CreateVertexArrays(1, out int buffer);
        return buffer;
    }

    public static void ClearContext()
    {
        GL.BindVertexArray(0);
        BufferBindedInContext = 0;
    }
}
