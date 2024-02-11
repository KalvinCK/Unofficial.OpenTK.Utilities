using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferStructuredDynamic<T>(BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw) : IBufferObject, IDisposable
    where T : struct
{
    public BufferStructuredDynamic(int initialCount, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initialCount);
    }

    public BufferStructuredDynamic(T[] initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredDynamic(Span<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredDynamic(ReadOnlySpan<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredDynamic(List<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public BufferStructuredDynamic(IReadOnlyList<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ToAllocate(initStructuredCollection);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public BufferUsageHint UsageHint { get; private set; } = BufferUsageHint;

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public bool Allocated { get; private set; } = false;

    public int Count { get; protected set; }

    public int MemorySize => this.Count * this.Stride;

    #region Allocate
    public void ToAllocate(int count)
    {
        this.Allocated = true;
        this.Count = count;
        GL.NamedBufferData(this.BufferID, this.MemorySize, IntPtr.Zero, this.UsageHint);
    }

    public void ToAllocate(T[] structuredCollection)
    {
        this.Allocated = true;
        this.Count = structuredCollection.Length;
        GL.NamedBufferData(this.BufferID, this.MemorySize, ref structuredCollection[0], this.UsageHint);
    }

    public void ToAllocate(Span<T> structuredCollection)
    {
        this.Allocated = true;
        this.Count = structuredCollection.Length;
        GL.NamedBufferData(this.BufferID, this.MemorySize, ref structuredCollection[0], this.UsageHint);
    }

    public unsafe void ToAllocate(ReadOnlySpan<T> structuredCollection)
    {
        this.Allocated = true;
        this.Count = structuredCollection.Length;
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferData(this.BufferID, this.MemorySize, (IntPtr)ptr, this.UsageHint);
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
        fixed (void* ptr = &structuredCollection[0])
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

    public void Update(int indexStructure, T structuredCollection)
    {
        GL.NamedBufferSubData(this.BufferID, indexStructure * this.Stride, this.Stride, ref structuredCollection);
    }
    #endregion

    public T GetData(int index)
    {
        var data = default(T);
        GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref data);
        return data;
    }

    public T[] GetData(int index, int indexFirst)
    {
        T[] data = new T[indexFirst - index];
        GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, indexFirst * this.Stride, data);
        return data;
    }

    public T[] GetAllData()
    {
        T[] data = new T[this.Count];
        GL.GetNamedBufferSubData(this.BufferID, 0, this.MemorySize, data);
        return data;
    }

    public void Clear()
    {
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, IntPtr.Zero);
    }

    public void Clear(int indexStructure)
    {
        GL.NamedBufferSubData(this.BufferID, indexStructure * this.Stride, this.Stride, IntPtr.Zero);
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
            this.UsageHint = 0;
            this.Allocated = false;
        }
    }
}
