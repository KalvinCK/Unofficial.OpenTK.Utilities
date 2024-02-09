using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class RenderBufferObject : IBuffer, IDisposable
{
    public int BufferID { get; private set; }
    public static int BufferBindedInContext { get; private set; }
    public RenderBufferObject()
    {
        GL.CreateRenderbuffers(1, out int buffer);
        BufferID = buffer;
    }

    public void Bind()
    {
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, BufferID);
        BufferBindedInContext = BufferID;
    }
    public static void ClearContext()
    {
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        BufferBindedInContext = 0;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteRenderbuffer(BufferID);
            BufferID = 0;
        }
    }
    public void Storage(RenderbufferStorage renderbuffer, int width, int height)
    {
        GL.NamedRenderbufferStorage(BufferID, renderbuffer, width, height);
    }
    public void StorageMultisampler(RenderbufferStorage renderbuffer, int width, int height, int samples)
    {
        GL.NamedRenderbufferStorageMultisample(BufferID, samples, renderbuffer, width, height);
    }
    public unsafe void GetParameter(RenderbufferParameterName parameterName, out int value)
    {
        GL.GetNamedRenderbufferParameter(BufferID, parameterName, out value);
    }
}
