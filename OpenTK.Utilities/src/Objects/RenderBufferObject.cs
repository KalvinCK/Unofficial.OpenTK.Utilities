using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class RenderbufferObject : IReadOnlyRenderbufferObject, IDisposable
{
    public RenderbufferObject()
    {
        this.BufferID = IReadOnlyRenderbufferObject.CreateBuffer();
    }

    public int BufferID { get; private set; }

    public void Bind()
    {
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.BufferID);
    }

    public void Storage(RenderbufferStorage RenderbufferStorage, int width, int height)
    {
        GL.NamedRenderbufferStorage(this.BufferID, RenderbufferStorage, width, height);
    }

    public void StorageMultisampler(RenderbufferStorage RenderbufferStorage, int width, int height, int samples)
    {
        GL.NamedRenderbufferStorageMultisample(this.BufferID, samples, RenderbufferStorage, width, height);
    }

    public unsafe void GetParameter(RenderbufferParameterName RenderbufferParameterName, out int value)
    {
        GL.GetNamedRenderbufferParameter(this.BufferID, RenderbufferParameterName, out value);
    }

    public unsafe void GetParameter(RenderbufferParameterName RenderbufferParameterName, int[] values)
    {
        GL.GetNamedRenderbufferParameter(this.BufferID, RenderbufferParameterName, values);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteRenderbuffer(this.BufferID);
            this.BufferID = 0;
        }
    }
}
