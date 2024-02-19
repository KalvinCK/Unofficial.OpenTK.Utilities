using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.BufferObjects;

namespace OpenTK.Utilities.Objects;

public class VertexArrayObject : IVertexArrayObject, IDisposable
{
    public VertexArrayObject()
    {
        this.BufferID = IVertexArrayObject.CreateBuffer();
    }

    public int BufferID { get; private set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteVertexArray(this.BufferID);
            this.BufferID = 0;
        }
    }

    public void Bind()
    {
        GL.BindVertexArray(this.BufferID);
        IVertexArrayObject.BufferBindedInContext = this.BufferID;
    }

    public void SetElementBuffer<TBuffer>(TBuffer BufferObject)
        where TBuffer : IBuffer
    {
        GL.VertexArrayElementBuffer(this.BufferID, BufferObject.BufferID);
    }

    public void AddVertexBuffer<TBuffer>(int BindingIndex, TBuffer BufferObject, int BufferOffset = 0)
        where TBuffer : IBufferObject
    {
        GL.VertexArrayVertexBuffer(this.BufferID, BindingIndex, BufferObject.BufferID, BufferOffset, BufferObject.Stride);
    }

    public void AddVertexBuffer<TBuffer>(int BindingIndex, TBuffer BufferObject, int VertexStride, int BufferOffset = 0)
        where TBuffer : IBuffer
    {
        GL.VertexArrayVertexBuffer(this.BufferID, BindingIndex, BufferObject.BufferID, BufferOffset, VertexStride);
    }

    public void SetAttribFormat(int BindingIndex, int AttribIndex, int Count, VertexAttribType VertexAttribType, int RelativeOffset, bool Normalize = false)
    {
        GL.EnableVertexArrayAttrib(this.BufferID, AttribIndex);
        GL.VertexArrayAttribFormat(this.BufferID, AttribIndex, Count, VertexAttribType, Normalize, RelativeOffset);
        GL.VertexArrayAttribBinding(this.BufferID, AttribIndex, BindingIndex);
    }

    public void SetAttribFormatI(int bindingIndex, int attribIndex, int attribTypeElements, VertexAttribType VertexAttribType, int relativeOffset)
    {
        GL.EnableVertexArrayAttrib(this.BufferID, attribIndex);
        GL.VertexArrayAttribIFormat(this.BufferID, attribIndex, attribTypeElements, VertexAttribType, relativeOffset);
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
}
