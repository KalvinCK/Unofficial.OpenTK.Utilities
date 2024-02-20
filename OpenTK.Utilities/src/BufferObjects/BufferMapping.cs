using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

/// <summary>
/// Represents the all mapping region of a buffer for read and write operations.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class BufferMapping<T> : IBufferObject, IDisposable
    where T : struct
{
    private const BufferAccessMask AccessFlags = BufferAccessMask.MapReadBit | BufferAccessMask.MapWriteBit | BufferAccessMask.MapPersistentBit | BufferAccessMask.MapCoherentBit;

    private int count = 0;

    public BufferMapping(int count = 1)
    {
        this.Count = count;
    }

    public BufferMapping(params T[] values)
    {
        this.ConfigureBuffer(values);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int BufferID { get; private set; }

    public IntPtr PtrRegion { get; private set; }

    public int MemorySize => this.count * this.Stride;

    public int Count
    {
        get => this.count;
        set
        {
            value = Math.Max(1, value);

            if (value == this.count)
            {
                return;
            }

            T[] bufferDatas = [];

            if (this.HasAllocated)
            {
                bufferDatas = this.ExtractCollection(0, value);
                if (bufferDatas.Length != value)
                {
                    Array.Resize(ref bufferDatas, value);
                }

                this.Dispose(true);
            }
            else
            {
                Array.Resize(ref bufferDatas, value);
            }

            this.ConfigureBuffer(bufferDatas);
            Array.Clear(bufferDatas);
        }
    }

    private int EndIndex => this.count - 1;

    private bool HasAllocated { get; set; }

    public unsafe ref T this[int index]
    {
        get
        {
            this.ValidateIndex(index);
            IntPtr indexPtr = IntPtr.Add(this.PtrRegion, index * this.Stride);
            return ref Unsafe.AsRef<T>(indexPtr.ToPointer());
        }
    }

    #region Replace
    public unsafe void Replace(int index, T[] structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int sizeValid = this.ValidateOffset(index, length);

        fixed (void* dataPtr = structuredCollection)
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(sizeValid * this.Stride));
        }
    }

    public unsafe void Replace(int index, Span<T> structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        // var ptr = MemoryMarshal.GetReference(structuredCollection);
        int sizeValid = this.ValidateOffset(index, length);

        fixed (void* dataPtr = structuredCollection)
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(sizeValid * this.Stride));
        }
    }

    public unsafe void Replace(int index, ReadOnlySpan<T> structuredCollection)
    {
        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int sizeValid = this.ValidateOffset(index, length);

        fixed (void* dataPtr = &structuredCollection[0])
        {
            IntPtr indexPtr = this.GetMemoryIndexPointer(index);
            Unsafe.CopyBlock(indexPtr.ToPointer(), dataPtr, (uint)(sizeValid * this.Stride));
        }
    }

    public unsafe void Replace(int index, List<T> structuredCollection)
        => this.Replace(index, CollectionsMarshal.AsSpan(structuredCollection));

    public unsafe void Replace(int index, IReadOnlyList<T> structuredCollection)
        => this.Replace(index, CollectionsMarshal.AsSpan(structuredCollection.ToList()));

    /// <summary>
    /// Leaves all data with the default value.
    /// </summary>
    public void Reset()
        => this.Replace(0, new T[this.Count]);

    #endregion

    #region Extract
    public void ExtractContents(int index, int size, out Span<T> spanValues)
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

    public void ExtractContents(out Span<T> spanValues)
        => this.ExtractContents(0, this.Count, out spanValues);

    public T[] ExtractCollection(int index, int size)
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

    public T[] ExtractCollection()
        => this.ExtractCollection(0, this.Count);
    #endregion

    #region ###

    public IEnumerator<T> GetEnumerator()
    {
        T[] datas = this.ExtractCollection();
        foreach (T data in datas)
        {
            yield return data;
        }
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

    public override string ToString()
    {
        return $"Stride: [{this.Stride}] Count of elements: [{this.Count}] Total capacity in bytes: [{this.MemorySize}]";
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
            GL.UnmapNamedBuffer(this.BufferID);
            GL.DeleteBuffer(this.BufferID);
            this.BufferID = 0;
            this.PtrRegion = IntPtr.Zero;
            this.count = 0;
            this.HasAllocated = false;
        }
    }

    #endregion

    #region Valids & Checks
    private void ConfigureBuffer(T[] bufferDatas)
    {
        this.BufferID = IBufferObject.CreateBuffer();
        this.count = bufferDatas.Length;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, bufferDatas, (BufferStorageFlags)AccessFlags);
        this.PtrRegion = GL.MapNamedBufferRange(this.BufferID, 0, this.MemorySize, AccessFlags);
        this.HasAllocated = true;
    }

    private IntPtr GetMemoryIndexPointer(int index)
    {
        return IntPtr.Add(this.PtrRegion, index * this.Stride);
    }

    private int ValidateOffset(int index, int size)
    {
        this.ValidateIndex(index);
        return index + size >= this.Count ? this.Count - index : size;
    }

    private void ValidateIndex(int index)
    {
        if ((index >= 0 && index <= this.EndIndex) is false)
        {
            throw new IndexOutOfRangeException("Index was outside the bounds of the buffer.");
        }
    }

    private bool CheckIsEmptyCollection(int count)
    {
        return count < 1;
    }
    #endregion
}
