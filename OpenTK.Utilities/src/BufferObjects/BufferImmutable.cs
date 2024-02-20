using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferImmutable<T>(StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit) : IBufferObject, IDisposable
    where T : struct
{
    private BufferStorageFlags flags = (BufferStorageFlags)StorageImmutable;

    public BufferImmutable(int initialCount, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
       : this(StorageImmutable)
    {
        this.ReserveData(initialCount);
    }

    public BufferImmutable(T[] initStructuredCollection, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
        : this(StorageImmutable)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferImmutable(Span<T> initStructuredCollection, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
        : this(StorageImmutable)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferImmutable(ReadOnlySpan<T> initStructuredCollection, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
        : this(StorageImmutable)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferImmutable(List<T> initStructuredCollection, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
        : this(StorageImmutable)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferImmutable(IReadOnlyList<T> initStructuredCollection, StorageUseFlag StorageImmutable = StorageUseFlag.DynamicStorageBit)
        : this(StorageImmutable)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    #region Props
    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public bool Allocated { get; private set; }

    public int Count { get; protected set; }

    public int MemorySize => this.Count * this.Stride;

    private int EndIndex => this.Count - 1;

    public T this[int index]
    {
        get
        {
            this.ValidateIndex(index);

            var data = default(T);
            GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref data);
            return data;
        }

        set
        {
            this.CheckHasAllocated();
            GL.NamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref value);
        }
    }
    #endregion

    #region Reserve
    public void ReserveData(int count)
    {
        this.CheckCanBeAllocated();

        if (count < 1)
        {
            throw new EmptyAllocationException();
        }

        this.Count = count;
        this.Allocated = true;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, IntPtr.Zero, this.flags);
    }

    public void ReserveAssignData(T[] structuredCollection)
    {
        this.CheckCanBeAllocated();
        this.CheckCountCollectionForAlocate(structuredCollection.Length);

        this.Count = structuredCollection.Length;
        this.Allocated = true;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, ref structuredCollection[0], this.flags);
    }

    public void ReserveAssignData(Span<T> structuredCollection)
    {
        this.CheckCanBeAllocated();
        this.CheckCountCollectionForAlocate(structuredCollection.Length);

        this.Count = structuredCollection.Length;
        this.Allocated = true;
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, ref structuredCollection[0], this.flags);
    }

    public unsafe void ReserveAssignData(ReadOnlySpan<T> structuredCollection)
    {
        this.CheckCanBeAllocated();
        this.CheckCountCollectionForAlocate(structuredCollection.Length);

        this.Count = structuredCollection.Length;
        this.Allocated = true;
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferStorage(this.BufferID, this.MemorySize, (IntPtr)ptr, this.flags);
        }
    }

    public void ReserveAssignData(List<T> structuredCollection)
        => this.ReserveAssignData(CollectionsMarshal.AsSpan(structuredCollection));

    public void ReserveAssignData(IReadOnlyList<T> structuredCollection)
        => this.ReserveAssignData(structuredCollection.ToList());

    #endregion

    #region Replace
    public void ReplaceSubData(int index, T[] structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int count = this.ValidateOffset(index, length);
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, count * this.Stride, structuredCollection);
    }

    public void ReplaceSubData(int index, Span<T> structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int count = this.ValidateOffset(index, length);
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, count * this.Stride, ref structuredCollection[0]);
    }

    public unsafe void ReplaceSubData(int index, ReadOnlySpan<T> structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int count = this.ValidateOffset(index, length);
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferSubData(this.BufferID, index * this.Stride, count * this.Stride, (IntPtr)ptr);
        }
    }

    public void ReplaceSubData(int index, List<T> structuredCollection)
        => this.ReplaceSubData(index, CollectionsMarshal.AsSpan(structuredCollection));

    public void ReplaceSubData(int index, IReadOnlyList<T> structuredCollection)
        => this.ReplaceSubData(index, structuredCollection.ToList());

    public void ReplaceSubData(T[] structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public void ReplaceSubData(Span<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public unsafe void ReplaceSubData(ReadOnlySpan<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public void ReplaceSubData(List<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public void ReplaceSubData(IReadOnlyList<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    #endregion

    #region Extract
    public T[] ExtractContents(int index, int size)
    {
        this.CheckHasAllocated();

        T[] data = new T[this.ValidateOffset(index, size)];
        GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, data.Length * this.Stride, data);
        return data;
    }

    public T[] ExtractCollection()
    {
        return this.ExtractContents(0, this.Count);
    }
    #endregion

    #region Clear
    public void ClearOffset(int index, int size)
    {
        if (!this.Allocated)
        {
            return;
        }

        using var emptyData = new EmptyData<T>(this.ValidateOffset(index, size));
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, emptyData.Count * this.Stride, emptyData.Data);
    }

    public void Clear()
        => this.ClearOffset(0, this.Count);
    #endregion

    #region Mapping
    public MappedRegion<T> GetMapping() => this.GetMapping(0, this.Count);

    public MappedRegion<T> GetMapping(int index, int size)
    {
        this.CheckHasAllocated();
        return new MappedRegion<T>(this, index, this.ValidateOffset(index, size));
    }

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
            GL.DeleteBuffer(this.BufferID);
            this.BufferID = 0;
            this.Count = 0;
            this.flags = 0;
            this.Allocated = false;
        }
    }
    #endregion

    #region Valids & Checks
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

    private void CheckCountCollectionForAlocate(int count)
    {
        if (count < 1)
        {
            throw new EmptyCollectionException();
        }
    }

    private bool CheckIsEmptyCollection(int count)
    {
        return count < 1;
    }

    private void CheckCanBeAllocated()
    {
        if (this.Allocated)
        {
            throw new StaticBufferAllocationException();
        }
    }

    private void CheckHasAllocated()
    {
        if (this.Allocated is false)
        {
            throw new UnallocatedBufferException();
        }
    }
    #endregion

}
