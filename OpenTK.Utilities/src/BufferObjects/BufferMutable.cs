using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferMutable<T>(BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw) : IReadOnlyBufferObject, IDisposable
    where T : struct
{
    public BufferMutable(int initialCount, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.Reserve(initialCount);
    }

    public BufferMutable(T[] initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferMutable(Span<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferMutable(ReadOnlySpan<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint)
    {
        this.ReserveAssignData(initStructuredCollection);
    }

    public BufferMutable(List<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint) => this.ReserveAssignData(initStructuredCollection);

    public BufferMutable(IReadOnlyList<T> initStructuredCollection, BufferUsageHint BufferUsageHint = BufferUsageHint.DynamicDraw)
        : this(BufferUsageHint) => this.ReserveAssignData(initStructuredCollection);

    #region Props
    public int Stride { get; } = Unsafe.SizeOf<T>();

    public BufferUsageHint UsageHint { get; private set; } = BufferUsageHint;

    public int BufferID { get; private set; } = IReadOnlyBufferObject.CreateBuffer();

    public bool Allocated { get; private set; }

    public int Count { get; protected set; }

    public int MemorySize => this.Count * this.Stride;

    private int EndIndex => this.Count - 1;

    public T this[int index]
    {
        get
        {
            if (index >= 0 && index <= this.EndIndex)
            {
                this.ValidateIndex(index);
                var data = default(T);
                GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref data);
                return data;
            }
            else
            {
                throw new Exception("The index is out of range.");
            }
        }

        set
        {
            this.CheckHasAllocated();
            GL.NamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref value);
        }
    }
    #endregion

    #region Allocate
    public void Reserve(int count)
    {
        if (count < 1)
        {
            throw new EmptyAllocationException();
        }

        this.Count = count;
        this.Allocated = true;
        GL.NamedBufferData(this.BufferID, this.MemorySize, IntPtr.Zero, this.UsageHint);
    }

    public void ReserveAssignData(T[] structuredCollection)
    {
        this.CheckCountCollectionForAlocate(structuredCollection.Length);
        this.Count = structuredCollection.Length;
        this.Allocated = true;
        GL.NamedBufferData(this.BufferID, this.MemorySize, ref structuredCollection[0], this.UsageHint);
    }

    public void ReserveAssignData(Span<T> structuredCollection)
    {
        this.CheckCountCollectionForAlocate(structuredCollection.Length);
        this.Count = structuredCollection.Length;
        this.Allocated = true;
        GL.NamedBufferData(this.BufferID, this.MemorySize, ref structuredCollection[0], this.UsageHint);
    }

    public unsafe void ReserveAssignData(ReadOnlySpan<T> structuredCollection)
    {
        this.CheckCountCollectionForAlocate(structuredCollection.Length);
        this.Count = structuredCollection.Length;
        this.Allocated = true;
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferData(this.BufferID, this.MemorySize, (IntPtr)ptr, this.UsageHint);
        }
    }

    public void ReserveAssignData(List<T> structuredCollection)
    {
        this.ReserveAssignData(CollectionsMarshal.AsSpan(structuredCollection));
    }

    public void ReserveAssignData(IReadOnlyList<T> structuredCollection)
    {
        this.ReserveAssignData(structuredCollection.ToList());
    }
    #endregion

    #region Replace
    public unsafe void ReplaceSubData(int index, T[] structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int offset = this.ValidateOffset(index, length);
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, offset * this.Stride, structuredCollection);
    }

    public void ReplaceSubData(int index, Span<T> structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int offset = this.ValidateOffset(index, length);
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, offset * this.Stride, ref structuredCollection[0]);
    }

    public unsafe void ReplaceSubData(int index, ReadOnlySpan<T> structuredCollection)
    {
        this.CheckHasAllocated();

        int length = structuredCollection.Length;
        if (this.CheckIsEmptyCollection(length))
        {
            return;
        }

        int offset = this.ValidateOffset(index, length);
        fixed (void* ptr = &structuredCollection[0])
        {
            GL.NamedBufferSubData(this.BufferID, index * this.Stride, offset * this.Stride, (IntPtr)ptr);
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

    public void ReplaceSubData(ReadOnlySpan<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public void ReplaceSubData(List<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    public void ReplaceSubData(IReadOnlyList<T> structuredCollection)
        => this.ReplaceSubData(0, structuredCollection);

    #endregion

    #region Extract
    public T[] ExtractCollection(int index, int size)
    {
        this.CheckHasAllocated();

        T[] data = new T[this.ValidateOffset(index, size)];
        GL.GetNamedBufferSubData(this.BufferID, index * this.Stride, data.Length * this.Stride, data);
        return data;
    }

    public T[] ExtractCollection() => this.ExtractCollection(0, this.Count);

    #endregion

    #region Clear
    public void ClearOffset(int index, int size)
    {
        this.CheckHasAllocated();
        GL.NamedBufferSubData(this.BufferID, index * this.Stride, size * this.Stride, IntPtr.Zero);
    }

    public void Clear()
        => this.ClearOffset(0, this.Count);

    #endregion

    #region Mapping
    public MappedRegion<T> GetMapping() => this.GetMapping(0, this.Count);

    public MappedRegion<T> GetMapping(int index, int size)
    {
        this.CheckHasAllocated();
        Console.WriteLine(this.ValidateOffset(index, size));
        return new MappedRegion<T>(this, index, this.ValidateOffset(index, size - 1));
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
            this.UsageHint = 0;
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

    private void CheckHasAllocated()
    {
        if (this.Allocated is false)
        {
            throw new UnallocatedBufferException();
        }
    }
    #endregion
}
