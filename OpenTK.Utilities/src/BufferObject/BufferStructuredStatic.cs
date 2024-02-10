using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferStructuredStatic<T>(BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit) : 
    IBufferObject, IDisposable  where T : struct
{
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public string DebugName { get; set; } = IBufferObject.Unnamed;
    public BufferStorageFlags StorageFlags { get; private set; } = BufferStorageFlags;
    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();
    public int Count { get; protected set; }
    public int MemorySize => Count * Stride;

    #region Init
    public BufferStructuredStatic(int initialCount, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit) 
        : this(BufferStorageFlags) 
    {
        ToAllocate(initialCount);
    }
    public BufferStructuredStatic(T[] initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(Span<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(ReadOnlySpan<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(List<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredStatic(IReadOnlyList<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags) 
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
    public BufferStructuredStatic<T> Clone()
    {
        var newBuffer = new BufferStructuredStatic<T>(StorageFlags);
        Copy(newBuffer, 0, 0, Count);

        return newBuffer;
    }
    public void Bind(BufferTarget BufferTarget)
    {
        GL.BindBuffer(BufferTarget, BufferID);
    }
    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int BindingIndex)
    {
        GL.BindBufferBase(BufferRangeTarget, BindingIndex, BufferID);
    }
    public override string ToString()
    {
        return $"Stride: [{Stride}] Count of elements: [{Count}] Total capacity in bytes: [{MemorySize}]\n";
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
            BufferID = 0;
            Count = 0;
            DebugName = IBufferObject.Unnamed;
            StorageFlags = BufferStorageFlags.None;
        }
    }
}
