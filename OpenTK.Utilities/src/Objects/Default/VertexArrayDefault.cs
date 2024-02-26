using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public readonly struct VertexArrayDefault : IReadOnlyVertexArrayObject
{
    public int BufferID { get; }

    public void Bind()
    {
        GL.BindVertexArray(this.BufferID);
    }
}
