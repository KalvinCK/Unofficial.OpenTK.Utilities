using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferUnstructured(BufferTarget BufferTarget) : IBuffer, IDisposable
{
    public BufferTarget Target { get; } = BufferTarget;
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int MemoryBytesSize { get; protected set; }
    public void ToAllocateStaticMemory(int bytesSize, IntPtr Data, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
    {
        MemoryBytesSize = bytesSize;
        GL.NamedBufferStorage(BufferID, MemoryBytesSize, Data, BufferStorageFlags);
    }
    public void ToAllocateDynamicMemory(int bytesSize, IntPtr Data, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
    {
        MemoryBytesSize = bytesSize;
        GL.NamedBufferData(BufferID, MemoryBytesSize, Data, BufferUsageHint);
    }
    public unsafe void UpdateMemory(int bytesOffset, int bytesSize, IntPtr DataPointer)
    {
        GL.NamedBufferSubData(BufferID, bytesOffset, bytesSize, DataPointer);
    }
    public unsafe void UpdateMemory(int bytesOffset, int bytesSize, void* DataPointer)
    {
        UpdateMemory(bytesOffset, bytesSize, (IntPtr)DataPointer);
    }
    public void UpdateMemory<T>(int bytesOffset, int bytesSize, T Data) where T : unmanaged
    {
        GL.NamedBufferSubData(BufferID, bytesOffset, bytesSize, ref Data);
    }
    public void UpdateMemory<T>(int bytesOffset, int bytesSize, T[] Data) where T : unmanaged
    {
        GL.NamedBufferSubData(BufferID, bytesOffset, bytesSize, ref Data[0]);
    }
    public void UpdateMemory<T>(int bytesOffset, int bytesSize, Span<T> Data) where T : unmanaged
    {
        GL.NamedBufferSubData(BufferID, bytesOffset, bytesSize, ref Data[0]);
    }
    public unsafe void UpdateMemory<T>(int bytesOffset, int bytesSize, ReadOnlySpan<T> Data) where T : unmanaged
    {
        fixed (void* ptr = &Data[0])
        {
            UpdateMemory(bytesOffset, bytesSize, ptr);
        }
    }

    public void Clear(int bytesOffset, int bytesSize)
    {
        UpdateMemory(bytesOffset, bytesSize, IntPtr.Zero);
    }

    public virtual void Copy<TBuffer>(TBuffer writerBuffer, int readOffsetInBytes, int writeOffsetInBytes, int bytesSize) where TBuffer : IBufferObject
    {
        GL.CopyNamedBufferSubData(BufferID, writerBuffer.BufferID, readOffsetInBytes, writeOffsetInBytes, bytesSize);
    }

    public void Bind()
    {
        GL.BindBuffer(Target, BufferID);
    }
    public void BindBufferBase(int bindingIndex)
    {
        GL.BindBufferBase((BufferRangeTarget)Target, bindingIndex, BufferID);
    }
    public void ClearContext()
    {
        GL.BindBuffer(Target, 0);
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
            GL.DeleteBuffer(BufferID);
        }
    }
}
