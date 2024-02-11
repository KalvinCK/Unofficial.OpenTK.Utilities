using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

internal interface IRenderBufferObject
{
    public static int BufferBindedInContext { get; internal set; }

    public int BufferID { get; }

    public void Bind();

    public unsafe void GetParameter(RenderbufferParameterName parameterName, out int value);

    public unsafe void GetParameter(RenderbufferParameterName parameterName, int[] values);

    public static void ClearContext()
    {
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        BufferBindedInContext = 0;
    }

    internal static int CreateBuffer()
    {
        GL.CreateRenderbuffers(1, out int buffer);
        return buffer;
    }
}
