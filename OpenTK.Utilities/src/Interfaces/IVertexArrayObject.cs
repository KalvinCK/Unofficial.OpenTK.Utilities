using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public interface IVertexArrayObject : IBuffer
{
    public void Bind();
    public static int BufferBindedInContext { get; internal set; }
    public static void ClearContext()
    {
        GL.BindVertexArray(0);
        BufferBindedInContext = 0;
    }

    public static readonly IVertexArrayObject Empty = new VertexArrayObject();
}
