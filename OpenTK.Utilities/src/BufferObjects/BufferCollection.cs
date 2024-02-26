using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferCollection<T> : IEnumerable<T>, IReadOnlyBufferObject, IDisposable
    where T : struct
{
    private List<T> collectionInternal = [];

    public int BufferID { get; private set; } = IReadOnlyBufferObject.CreateBuffer();

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int Count => this.collectionInternal.Count;

    public int MemorySize => this.Count * this.Stride;

    public virtual T this[int index]
    {
        get => this.collectionInternal[index];
        set
        {
            T item = value;
            this.collectionInternal[index] = item;
            GL.NamedBufferSubData(this.BufferID, index * this.Stride, this.Stride, ref item);
        }
    }

    public int RemoveAll(Predicate<T> match)
    {
        int result = this.collectionInternal.RemoveAll(match);
        if (result != 0)
        {
            this.UpdateCollectionBuffer();
        }

        return result;
    }

    public bool Contains(T item)
    {
        return this.collectionInternal.Contains(item);
    }

    public int IndexOf(T item)
    {
        return this.collectionInternal.IndexOf(item);
    }

    public virtual void Add(T item)
    {
        this.collectionInternal.Add(item);
        this.UpdateCollectionBuffer();
    }

    public virtual void Clear()
    {
        this.collectionInternal.Clear();
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, IntPtr.Zero);
    }

    public virtual void RemoveAt(int index)
    {
        this.collectionInternal.RemoveAt(index);
        this.UpdateCollectionBuffer();
    }

    public virtual bool Remove(T item)
    {
        var result = this.collectionInternal.Remove(item);
        if (result)
        {
            this.UpdateCollectionBuffer();
        }

        return result;
    }

    public virtual void Insert(int index, T item)
    {
        this.collectionInternal.Insert(index, item);
        this.UpdateCollectionBuffer();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this.collectionInternal.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.collectionInternal.GetEnumerator();
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
            this.collectionInternal = [];
            this.BufferID = 0;
        }
    }

    private void UpdateCollectionBuffer()
    {
        if (this.Count > 0)
        {
            var span = CollectionsMarshal.AsSpan(this.collectionInternal);
            GL.NamedBufferData(this.BufferID, this.MemorySize, ref span[0], BufferUsageHint.StreamDraw);
        }
    }
}
