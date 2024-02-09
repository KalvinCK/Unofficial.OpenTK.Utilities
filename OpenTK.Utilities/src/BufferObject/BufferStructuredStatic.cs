using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferStructuredStatic<T>(BufferTarget BufferTarget, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit) : 
    IBufferObject, IDisposable  where T : struct
{
    public BufferTarget Target { get; } = BufferTarget;
    public BufferStorageFlags StorageFlags { get; } = BufferStorageFlags;
    public string DebugName { get; set; } = "UNNAMED";
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public int Count { get; protected set; }
    public int MemorySize => Count * Stride;

    #region Init
    public BufferStructuredStatic(BufferTarget BufferTarget, int initialCount, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit) 
        : this(BufferTarget, BufferStorageFlags) 
    {
        ToAllocate(initialCount);
    }
    public BufferStructuredStatic(BufferTarget BufferTarget, T[] initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferTarget, BufferStorageFlags)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(BufferTarget BufferTarget, Span<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferTarget, BufferStorageFlags)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(BufferTarget BufferTarget, ReadOnlySpan<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferTarget, BufferStorageFlags) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(BufferTarget BufferTarget, List<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferTarget, BufferStorageFlags) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(BufferTarget BufferTarget, IReadOnlyList<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferTarget, BufferStorageFlags) 
    {
        ToAllocate(initStructuredCollection);
    }
    #endregion

    #region Allocate
    public void ToAllocate(int count)
    {
        Count = count;
        GL.NamedBufferStorage(BufferID, MemorySize, IntPtr.Zero, StorageFlags);
    }
    public void ToAllocate(T[] structuredCollection)
    {
        Count = structuredCollection.Length;
        GL.NamedBufferStorage(BufferID, MemorySize, ref structuredCollection[0], StorageFlags);
    }
    public void ToAllocate(Span<T> structuredCollection)
    {
        Count = structuredCollection.Length;
        GL.NamedBufferStorage(BufferID, MemorySize, ref structuredCollection[0], StorageFlags);
    }
    public unsafe void ToAllocate(ReadOnlySpan<T> structuredCollection)
    {
        Count = structuredCollection.Length;

        fixed (T* ptr = structuredCollection)
        {
            GL.NamedBufferStorage(BufferID, MemorySize, (IntPtr)ptr, StorageFlags);
        }
    }
    public void ToAllocate(List<T> structuredCollection)
    {
        ToAllocate(CollectionsMarshal.AsSpan(structuredCollection));
    }
    public void ToAllocate(IReadOnlyList<T> structuredCollection)
    {
        ToAllocate((List<T>)structuredCollection);
    }
    #endregion

    #region Update
    public void Update(int indexStructure, T structuredCollection)
    {
        GL.NamedBufferSubData(BufferID, indexStructure * Stride, Stride, ref structuredCollection);
    }
    public void Update(T[] structuredCollection)
    {
        GL.NamedBufferSubData(BufferID, 0, MemorySize, ref structuredCollection[0]);
    }
    public void Update(Span<T> structuredCollection)
    {
        GL.NamedBufferSubData(BufferID, 0, MemorySize, ref structuredCollection[0]);
    }
    public unsafe void Update(ReadOnlySpan<T> structuredCollection)
    {
        fixed (T* ptr = structuredCollection)
        {
            GL.NamedBufferSubData(BufferID, 0, MemorySize, (IntPtr)ptr);
        }
    }
    public void Update(List<T> structuredCollection)
    {
        Update(CollectionsMarshal.AsSpan(structuredCollection));
    }
    public void Update(IReadOnlyList<T> structuredCollection)
    {
        Update(CollectionsMarshal.AsSpan((List<T>)structuredCollection));
    }
    #endregion

    public void Clear()
    {
        GL.NamedBufferSubData(BufferID, 0, MemorySize, IntPtr.Zero);
    }
    public void Clear(int indexStructure)
    {
        GL.NamedBufferSubData(BufferID, indexStructure * Stride, Stride, IntPtr.Zero);
    }

    public void Copy<TBuffer>(TBuffer writeBuffer, int readIndex, int writeIndex, int endIndex) where TBuffer : IBufferObject
    {
        GL.CopyNamedBufferSubData(BufferID, writeBuffer.BufferID, readIndex * Stride, writeIndex * Stride, endIndex * Stride);
    }
    public BufferStructuredStatic<T> CloneGPU()
    {
        var newBuffer = new BufferStructuredStatic<T>(Target, StorageFlags);
        Copy(newBuffer, 0, 0, Count);

        return newBuffer;
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
    public override string ToString()
    {
        return $"Target: [{Target}] Stride: [{Stride}] Count of elements: [{Count}] Total capacity in bytes: [{MemorySize}]\n";
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
            Count = 0;
        }
    }
}
