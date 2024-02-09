using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferStructuredDynamic<T>(BufferTarget BufferTarget, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw) : 
    IBufferObject, IDisposable where T : struct
{
    public BufferTarget Target { get; } = BufferTarget;
    public BufferUsageHint UsageHint { get; } = BufferUsageHint;
    public string DebugName { get; set; } = "UNNAMED";
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public int Count { get; protected set; }
    public int MemorySize => Count * Stride;

    #region Init
    public BufferStructuredDynamic(BufferTarget BufferTarget, int initialCount, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw) 
        : this(BufferTarget, BufferUsageHint) 
    {
        ToAllocate(initialCount);
    }
    public BufferStructuredDynamic(BufferTarget BufferTarget, T[] initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferTarget, BufferUsageHint)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredDynamic(BufferTarget BufferTarget, Span<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferTarget, BufferUsageHint)
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredDynamic(BufferTarget BufferTarget, ReadOnlySpan<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferTarget, BufferUsageHint) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredDynamic(BufferTarget BufferTarget, List<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferTarget, BufferUsageHint) 
    {
        ToAllocate(initStructuredCollection);
    }
    public BufferStructuredDynamic(BufferTarget BufferTarget, IReadOnlyList<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferTarget, BufferUsageHint) 
    {
        ToAllocate(initStructuredCollection);
    }
    #endregion

    #region Allocate
    public void ToAllocate(int count)
    {
        Count = count;
        GL.NamedBufferData(BufferID, MemorySize, IntPtr.Zero, UsageHint);
    }
    public void ToAllocate(T[] structuredCollection)
    {
        Count = structuredCollection.Length;
        GL.NamedBufferData(BufferID, MemorySize, ref structuredCollection[0], UsageHint);
    }
    public void ToAllocate(Span<T> structuredCollection)
    {
        Count = structuredCollection.Length;
        GL.NamedBufferData(BufferID, MemorySize, ref structuredCollection[0], UsageHint);
    }
    public unsafe void ToAllocate(ReadOnlySpan<T> structuredCollection)
    {
        Count = structuredCollection.Length;
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferData(BufferID, MemorySize, (IntPtr)ptr, UsageHint);
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
        fixed (void* ptr = &structuredCollection[0])
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
    public void Update(int indexStructure, T structuredCollection)
    {
        GL.NamedBufferSubData(BufferID, indexStructure * Stride, Stride, ref structuredCollection);
    }
    #endregion

    #region ###
    public T GetData(int index)
    {
        var data = new T();
        GL.GetNamedBufferSubData(BufferID, index * Stride, Stride, ref data);
        return data;
    }
    public T[] GetData(int index, int indexFirst)
    {
        T[] data = new T[indexFirst - index];
        GL.GetNamedBufferSubData(BufferID, index * Stride, indexFirst * Stride, data);
        return data;
    }
    public T[] GetAllData()
    {
        T[] data = new T[Count];
        GL.GetNamedBufferSubData(BufferID, 0, MemorySize, data);
        return data;
    }

    public void Clear()
    {
        GL.NamedBufferSubData(BufferID, 0, MemorySize, IntPtr.Zero);
    }
    public void Clear(int indexStructure)
    {
        GL.NamedBufferSubData(BufferID, indexStructure * Stride, Stride, IntPtr.Zero);
    }
    #endregion

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
