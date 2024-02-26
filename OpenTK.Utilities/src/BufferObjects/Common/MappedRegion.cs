using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.GL;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

/// <summary>
/// Represents all or part of the mapped region of a buffer for read and write operations.
/// </summary>
/// <remarks>
/// It is crucial that after the changes occur, the disposal '<see cref="Dispose"/>' takes place.
/// Direct modifications to the buffer while this mapping is active may cause unexpected errors or program crashes.
/// </remarks>
/// <typeparam name="T">The type of data in the buffer.</typeparam>
public struct MappedRegion<T> : IDisposable
    where T : struct
{
    private int bufferID;
    private int index;

    public MappedRegion(IReadOnlyBufferObject bufferObject, int index, int size)
    {
        this.bufferID = bufferObject.BufferID;
        this.index = index;
        this.Count = size;

        BufferAccessMask accessFlags = BufferAccessMask.MapReadBit | BufferAccessMask.MapWriteBit | BufferAccessMask.MapFlushExplicitBit;
        this.PtrRegion = GL.MapNamedBufferRange(this.bufferID, index * this.Stride, this.RegionSize, accessFlags);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int Count { get; private set; }

    public readonly int RegionSize => this.Count * this.Stride;

    private readonly int EndIndex => this.Count - 1;

    private IntPtr PtrRegion { get; set; }

    public readonly unsafe ref T this[int index]
    {
        get
        {
            this.ValidateIndex(index);
            IntPtr indexPtr = IntPtr.Add(this.PtrRegion, index * this.Stride);
            return ref Unsafe.AsRef<T>(indexPtr.ToPointer());
        }
    }

    public void Dispose()
    {
        GL.FlushMappedNamedBufferRange(this.bufferID, 0, this.RegionSize);
        GL.UnmapNamedBuffer(this.bufferID);
        this.bufferID = 0;
        this.PtrRegion = IntPtr.Zero;
        this.Count = 0;
        this.index = 0;
    }

    #region Replace
    public readonly unsafe void Replace(int index, T[] structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int count = this.ValidateOffset(index, length);

        fixed (void* dataPtr = structuredCollection)
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(count * this.Stride));
        }
    }

    public readonly unsafe void Replace(int index, Span<T> structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        var ptr = MemoryMarshal.GetReference(structuredCollection);

        int count = this.ValidateOffset(index, length);

        fixed (void* dataPtr = structuredCollection)
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(count * this.Stride));
        }
    }

    public readonly unsafe void Replace(int index, ReadOnlySpan<T> structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int count = this.ValidateOffset(index, length);

        fixed (void* dataPtr = &structuredCollection[0])
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(count * this.Stride));
        }
    }

    public readonly unsafe void Replace(int index, List<T> structuredCollection)
        => this.Replace(index, CollectionsMarshal.AsSpan(structuredCollection));

    public readonly unsafe void Replace(int index, IReadOnlyList<T> structuredCollection)
        => this.Replace(index, CollectionsMarshal.AsSpan(structuredCollection.ToList()));

    #endregion

    #region Extract
    public readonly void ExtractContents(int index, int size, out Span<T> spanValues)
    {
        int count = this.ValidateOffset(index, size);

        unsafe
        {
            if (index != 0)
            {
                IntPtr indexPtr = this.GetMemoryIndexPointer(index);
                spanValues = new Span<T>(indexPtr.ToPointer(), count * this.Stride);
            }
            else
            {
                spanValues = new Span<T>(this.PtrRegion.ToPointer(), count * this.Stride);
            }
        }
    }

    public readonly void ExtractContents(out Span<T> spanValues)
        => this.ExtractContents(0, this.Count, out spanValues);

    public readonly T[] ExtractCollection(int index, int size)
    {
        int count = this.ValidateOffset(index, size);

        T[] datas = new T[count];

        unsafe
        {
            fixed (void* dataPtr = datas)
            {
                if (index != 0)
                {
                    IntPtr indexPtr = this.GetMemoryIndexPointer(index);
                    Unsafe.CopyBlock(dataPtr, indexPtr.ToPointer(), (uint)(count * this.Stride));
                }
                else
                {
                    Unsafe.CopyBlock(dataPtr, this.PtrRegion.ToPointer(), (uint)(count * this.Stride));
                }
            }
        }

        return datas;
    }

    public readonly T[] ExtractCollection()
        => this.ExtractCollection(0, this.Count);

    public readonly IEnumerator<T> GetEnumerator()
    {
        T[] datas = this.ExtractCollection();
        foreach (T data in datas)
        {
            yield return data;
        }
    }
    #endregion

    #region Valids & Checks
    private readonly IntPtr GetMemoryIndexPointer(int index)
    {
        return IntPtr.Add(this.PtrRegion, index * this.Stride);
    }

    private readonly int ValidateOffset(int index, int size)
    {
        this.ValidateIndex(index);
        return index + size >= this.Count ? this.Count - index : size;
    }

    private readonly void ValidateIndex(int index)
    {
        if ((index >= 0 && index <= this.EndIndex) is false)
        {
            throw new IndexOutOfRangeException("Index was outside the bounds of the buffer.");
        }
    }

    private readonly bool CheckIsEmptyCollection(int count)
    {
        return count < 1;
    }
    #endregion
}
