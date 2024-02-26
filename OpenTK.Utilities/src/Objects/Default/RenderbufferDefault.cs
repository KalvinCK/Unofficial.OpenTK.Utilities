using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public readonly struct RenderbufferDefault : IReadOnlyRenderbufferObject
{
    public int BufferID { get; }

    public void Bind()
    {
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.BufferID);
    }

    public unsafe void GetParameter(RenderbufferParameterName RenderbufferParameterName, out int value)
    {
        GL.GetNamedRenderbufferParameter(this.BufferID, RenderbufferParameterName, out value);
    }

    public unsafe void GetParameter(RenderbufferParameterName RenderbufferParameterName, int[] values)
    {
        GL.GetNamedRenderbufferParameter(this.BufferID, RenderbufferParameterName, values);
    }
}
