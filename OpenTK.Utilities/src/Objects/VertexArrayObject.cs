using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.BufferObjects;

namespace OpenTK.Utilities.Objects;

public class VertexArrayObject : IReadOnlyVertexArrayObject, IDisposable
{
    public VertexArrayObject()
    {
        this.BufferID = IReadOnlyVertexArrayObject.CreateBuffer();
    }

    public int BufferID { get; private set; }

    public void Bind()
    {
        GL.BindVertexArray(this.BufferID);
    }

    public void FixElementBuffer<TBuffer>(TBuffer bufferObject)
        where TBuffer : IReadOnlyBuffer
    {
        GL.VertexArrayElementBuffer(this.BufferID, bufferObject.BufferID);
    }

    public void FixVertexBuffer<TBuffer>(TBuffer bufferObject)
        where TBuffer : IReadOnlyBufferObject
    {
        GL.VertexArrayVertexBuffer(this.BufferID, 0, bufferObject.BufferID, 0, bufferObject.Stride);
    }

    public void IncludeVertexBuffer<TBuffer>(int bindingIndex, TBuffer bufferObject, int vertexStride, int bufferOffset = 0)
        where TBuffer : IReadOnlyBuffer
    {
        GL.VertexArrayVertexBuffer(this.BufferID, bindingIndex, bufferObject.BufferID, bufferOffset, vertexStride);
    }

    public void SetAttribFormat(int bindingIndex, int attribIndex, int Count, VertexAttribType type, int relativeOffset, bool normalize = false)
    {
        GL.EnableVertexArrayAttrib(this.BufferID, attribIndex);
        GL.VertexArrayAttribFormat(this.BufferID, attribIndex, Count, type, normalize, relativeOffset);
        GL.VertexArrayAttribBinding(this.BufferID, attribIndex, bindingIndex);
    }

    public void SetAttribFormatI(int bindingIndex, int attribIndex, int attribTypeElements, VertexAttribType type, int relativeOffset)
    {
        GL.EnableVertexArrayAttrib(this.BufferID, attribIndex);
        GL.VertexArrayAttribIFormat(this.BufferID, attribIndex, attribTypeElements, type, relativeOffset);
        GL.VertexArrayAttribBinding(this.BufferID, attribIndex, bindingIndex);
    }

    public void SetPerBufferDivisor(int bindingIndex, int divisor)
    {
        GL.VertexArrayBindingDivisor(this.BufferID, bindingIndex, divisor);
    }

    public void DisableVertexAttribute(int attribIndex)
    {
        GL.DisableVertexArrayAttrib(this.BufferID, attribIndex);
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
            GL.DeleteVertexArray(this.BufferID);
            this.BufferID = 0;
        }
    }
}
