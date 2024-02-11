using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferStructuredStatic<T>(BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit) : IBufferObject, IDisposable
    where T : struct
{
    public BufferStructuredStatic(int initialCount, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
       : this(BufferStorageFlags)
    {
        this.ToAllocate(initialCount);
    }

    public BufferStructuredStatic(T[] initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredStatic(Span<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredStatic(ReadOnlySpan<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredStatic(List<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredStatic(IReadOnlyList<T> initStructuredCollection, BufferStorageFlags BufferStorageFlags = BufferStorageFlags.DynamicStorageBit)
        : this(BufferStorageFlags)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public BufferStorageFlags StorageFlags { get; private set; } = BufferStorageFlags;

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public int Count { get; protected set; }

    public int MemorySize => this.Count * this.Stride;

    #region Allocate
    public void ToAllocate(int count)
    {
        this.Count = count;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, IntPtr.Zero, this.StorageFlags);
    }

    public void ToAllocate(T[] structuredCollection)
    {
        this.Count = structuredCollection.Length;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, ref structuredCollection[0], this.StorageFlags);
    }

    public void ToAllocate(Span<T> structuredCollection)
    {
        this.Count = structuredCollection.Length;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, ref structuredCollection[0], this.StorageFlags);
    }

    public unsafe void ToAllocate(ReadOnlySpan<T> structuredCollection)
    {
        this.Count = structuredCollection.Length;

        fixed (T* ptr = structuredCollection)
        {
            GL.NamedBufferStorage(this.BufferID, this.MemorySize, (IntPtr)ptr, this.StorageFlags);
        }
    }

    public void ToAllocate(List<T> structuredCollection)
    {
        this.ToAllocate(CollectionsMarshal.AsSpan(structuredCollection));
    }

    public void ToAllocate(IReadOnlyList<T> structuredCollection)
    {
        this.ToAllocate((List<T>)structuredCollection);
    }
    #endregion

    #region Update
    public void Update(int indexStructure, T structuredCollection)
    {
        GL.NamedBufferSubData(this.BufferID, indexStructure * this.Stride, this.Stride, ref structuredCollection);
    }

    public void Update(T[] structuredCollection)
    {
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, ref structuredCollection[0]);
    }

    public void Update(Span<T> structuredCollection)
    {
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, ref structuredCollection[0]);
    }

    public unsafe void Update(ReadOnlySpan<T> structuredCollection)
    {
        fixed (T* ptr = structuredCollection)
        {
            GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, (IntPtr)ptr);
        }
    }

    public void Update(List<T> structuredCollection)
    {
        this.Update(CollectionsMarshal.AsSpan(structuredCollection));
    }

    public void Update(IReadOnlyList<T> structuredCollection)
    {
        this.Update(CollectionsMarshal.AsSpan((List<T>)structuredCollection));
    }
    #endregion

    public void Clear()
    {
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, IntPtr.Zero);
    }

    public void Clear(int indexStructure)
    {
        GL.NamedBufferSubData(this.BufferID, indexStructure * this.Stride, this.Stride, IntPtr.Zero);
    }

    public void Copy<TBuffer>(TBuffer writeBuffer, int readIndex, int writeIndex, int endIndex)
        where TBuffer : IBufferObject
    {
        GL.CopyNamedBufferSubData(this.BufferID, writeBuffer.BufferID, readIndex * this.Stride, writeIndex * this.Stride, endIndex * this.Stride);
    }

    public BufferStructuredStatic<T> Clone()
    {
        var newBuffer = new BufferStructuredStatic<T>(this.StorageFlags);
        this.Copy(newBuffer, 0, 0, this.Count);

        return newBuffer;
    }

    public void Bind(BufferTarget BufferTarget)
    {
        GL.BindBuffer(BufferTarget, this.BufferID);
    }

    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int bindingIndex)
    {
        GL.BindBufferBase(BufferRangeTarget, bindingIndex, this.BufferID);
    }

    public override string ToString()
    {
        return $"Stride: [{this.Stride}] Count of elements: [{this.Count}] Total capacity in bytes: [{this.MemorySize}]\n";
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
            this.Count = 0;
            this.StorageFlags = BufferStorageFlags.None;
        }
    }
}
