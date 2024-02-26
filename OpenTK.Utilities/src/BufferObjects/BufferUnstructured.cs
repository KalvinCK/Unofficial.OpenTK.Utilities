using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

/// <summary>
/// An insecure buffer very different from the others.
/// </summary>
public class BufferUnstructured : IReadOnlyBuffer, IDisposable
{
    public int BufferID { get; private set; } = IReadOnlyBufferObject.CreateBuffer();

    public int MemorySize { get; protected set; }

    public void ReserveMutableMemory(int bytesSize, IntPtr Data, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
    {
        this.MemorySize = bytesSize;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, Data, BufferStorageFlags);
    }

    public void ReserveImmutableMemory(int bytesSize, IntPtr Data, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
    {
        this.MemorySize = bytesSize;
        GL.NamedBufferData(this.BufferID, this.MemorySize, Data, BufferUsageHint);
    }

    public unsafe void UpdateMemory(int bytesOffset, int bytesSize, IntPtr DataPointer)
    {
        GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, DataPointer);
    }

    public unsafe void UpdateMemory(int bytesOffset, int bytesSize, void* DataPointer)
    {
        this.UpdateMemory(bytesOffset, bytesSize, (IntPtr)DataPointer);
    }

    public void UpdateMemory<T>(int bytesOffset, int bytesSize, T Data)
        where T : unmanaged
    {
        GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, ref Data);
    }

    public void UpdateMemory<T>(int bytesOffset, int bytesSize, T[] Data)
        where T : unmanaged
    {
        GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, Data);
    }

    public void UpdateMemory<T>(int bytesOffset, int bytesSize, Span<T> Data)
        where T : unmanaged
    {
        GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, ref Data[0]);
    }

    public unsafe void UpdateMemory<T>(int bytesOffset, int bytesSize, ReadOnlySpan<T> Data)
        where T : unmanaged
    {
        fixed (void* ptr = &Data[0])
        {
            GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, (IntPtr)ptr);
        }
    }

    public void GetOffset(int bytesOffset, int bytesSize, IntPtr data)
    {
        GL.GetNamedBufferSubData(this.BufferID, bytesOffset, bytesSize, data);
    }

    public unsafe IntPtr Map(BufferAccess BufferAccess = BufferAccess.ReadWrite)
    {
        return GL.MapNamedBuffer(this.BufferID, BufferAccess);
    }

    public unsafe IntPtr MapRange(int bytesOffset, int bytesSize, BufferAccessMask BufferAccessMask)
    {
        return GL.MapNamedBufferRange(this.BufferID, bytesOffset, bytesSize, BufferAccessMask);
    }

    public void Copy<TBuffer>(TBuffer writerBuffer, int readOffsetInBytes, int writeOffsetInBytes, int bytesSize)
        where TBuffer : IReadOnlyBuffer
    {
        GL.CopyNamedBufferSubData(this.BufferID, writerBuffer.BufferID, readOffsetInBytes, writeOffsetInBytes, bytesSize);
    }

    public void Invalidate()
    {
        GL.InvalidateBufferData(this.BufferID);
    }

    public void Bind(BufferTarget BufferTarget)
    {
        GL.BindBuffer(BufferTarget, this.BufferID);
    }

    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int bindingIndex)
    {
        GL.BindBufferBase(BufferRangeTarget, bindingIndex, this.BufferID);
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
            GL.DeleteBuffer(this.BufferID);
            this.BufferID = 0;
            this.MemorySize = 0;
        }
    }
}
