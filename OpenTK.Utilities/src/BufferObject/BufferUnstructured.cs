using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferUnstructured : IBuffer, IDisposable
{
    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public int MemoryBytesSize { get; protected set; }

    public void ToAllocateStaticMemory(int bytesSize, IntPtr Data, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
    {
        this.MemoryBytesSize = bytesSize;
        GL.NamedBufferStorage(this.BufferID, this.MemoryBytesSize, Data, BufferStorageFlags);
    }

    public void ToAllocateDynamicMemory(int bytesSize, IntPtr Data, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
    {
        this.MemoryBytesSize = bytesSize;
        GL.NamedBufferData(this.BufferID, this.MemoryBytesSize, Data, BufferUsageHint);
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
        GL.NamedBufferSubData(this.BufferID, bytesOffset, bytesSize, ref Data[0]);
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
            this.UpdateMemory(bytesOffset, bytesSize, ptr);
        }
    }

    public void Clear(int bytesOffset, int bytesSize)
    {
        this.UpdateMemory(bytesOffset, bytesSize, IntPtr.Zero);
    }

    public void Copy<TBuffer>(TBuffer writerBuffer, int readOffsetInBytes, int writeOffsetInBytes, int bytesSize)
        where TBuffer : IBuffer
    {
        GL.CopyNamedBufferSubData(this.BufferID, writerBuffer.BufferID, readOffsetInBytes, writeOffsetInBytes, bytesSize);
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
        }
    }
}
