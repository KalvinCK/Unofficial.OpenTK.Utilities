using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class VertexArrayObject : IVertexArrayObject, IDisposable
{
    public int BufferID { get; private set; }
    public VertexArrayObject()
    {
        GL.CreateVertexArrays(1, out int buffer);
        BufferID = buffer;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    public virtual void Dispose(bool disposing)
    {
        if(disposing)
        {
            GL.DeleteVertexArray(BufferID);
            BufferID = 0;
        }
    }
    public void Bind()
    {
        GL.BindVertexArray(BufferID);
        IVertexArrayObject.BufferBindedInContext = BufferID;
    }
    
    public void SetElementBuffer<TBuffer>(TBuffer BufferObject) where TBuffer : IBuffer
    {
        GL.VertexArrayElementBuffer(BufferID, BufferObject.BufferID);
    }
    public void AddVertexBuffer<TBuffer>(int BindingIndex, TBuffer BufferObject, int BufferOffset = 0) where TBuffer : IBufferObject
    {
        GL.VertexArrayVertexBuffer(BufferID, BindingIndex, BufferObject.BufferID, BufferOffset, BufferObject.Stride);
    }
    public void AddVertexBuffer<TBuffer>(int BindingIndex, TBuffer BufferObject, int VertexStride, int BufferOffset = 0) where TBuffer : IBuffer
    {
        GL.VertexArrayVertexBuffer(BufferID, BindingIndex, BufferObject.BufferID, BufferOffset, VertexStride);
    }
    public void SetAttribFormat(int BindingIndex, int AttribIndex, int Count, VertexAttribType VertexAttribType, int RelativeOffset, bool Normalize = false)
    {
        GL.EnableVertexArrayAttrib(BufferID, AttribIndex);
        GL.VertexArrayAttribFormat(BufferID, AttribIndex, Count, VertexAttribType, Normalize, RelativeOffset);
        GL.VertexArrayAttribBinding(BufferID, AttribIndex, BindingIndex);
    }
    public void SetAttribFormatI(int bindingIndex, int attribIndex, int attribTypeElements, VertexAttribType vertexAttribType, int relativeOffset)
    {
        GL.EnableVertexArrayAttrib(BufferID, attribIndex);
        GL.VertexArrayAttribIFormat(BufferID, attribIndex, attribTypeElements, vertexAttribType, relativeOffset);
        GL.VertexArrayAttribBinding(BufferID, attribIndex, bindingIndex);
    }
    public void SetPerBufferDivisor(int bindingIndex, int divisor)
    {
        GL.VertexArrayBindingDivisor(BufferID, bindingIndex, divisor);
    }
    public void DisableVertexAttribute(int attribIndex)
    {
        GL.DisableVertexArrayAttrib(BufferID, attribIndex);
    }

    public static IVertexArrayObject Empty => IVertexArrayObject.Empty;
}

