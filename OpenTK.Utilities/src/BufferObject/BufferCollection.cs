using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;


public class BufferCollection<T>(BufferTarget bufferTarget) :
    IBufferObject, IDisposable where T : struct
{
    // Used to control the buffer structure
    private readonly List<T> CollectionInternal = [];

    public List<T> CollectionData { get { return CollectionInternal; } }
    public BufferTarget Target { get; } = bufferTarget;
    public BufferUsageHint UsageHint { get; } = BufferUsageHint.DynamicDraw;
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public int Count => CollectionInternal.Count;
    public int MemorySize => Count * Stride;
    private void UpdateCollectionBuffer()
    {
        if(Count > 0)
        {
            var span = CollectionsMarshal.AsSpan(CollectionInternal);
            GL.NamedBufferData(BufferID, MemorySize, ref span[0], UsageHint);
        }
    }
    public virtual T this[int index]
    {
        get => CollectionInternal[index];
        set
        {
            T item = value;
            CollectionInternal[index] = item;
            GL.NamedBufferSubData(BufferID, index * Stride, Stride, ref item);
        }
    }
    public int RemoveAll(Predicate<T> match)
    {
        int result = CollectionInternal.RemoveAll(match);
        if(result != 0)
        {
            UpdateCollectionBuffer();
        }

        return result;
    }
    public bool Contains(T item)
    {
        return CollectionInternal.Contains(item);
    }
    public int IndexOf(T item)
    {
        return CollectionInternal.IndexOf(item);
    }
    public IEnumerable<T> GetEnumrable()
    {
        return CollectionInternal;
    }

    public virtual void Add(T item)
    {
        CollectionInternal.Add(item);
        UpdateCollectionBuffer();
    }

    public virtual void Clear()
    {
        CollectionInternal.Clear();
        GL.NamedBufferSubData(BufferID, 0, MemorySize, IntPtr.Zero);
    }
    
    public virtual void RemoveAt(int index)
    {
        CollectionInternal.RemoveAt(index);
        UpdateCollectionBuffer();
    }

    public virtual bool Remove(T item)
    {
        var result = CollectionInternal.Remove(item);
        if(result)
        {
            UpdateCollectionBuffer();
        }

        return result;
    }
    public virtual void Insert(int index, T item)
    {
        CollectionInternal.Insert(index, item);
        UpdateCollectionBuffer();

    }
    public void Bind()
    {
        GL.BindBuffer(Target, BufferID);
    }
    public void BindBufferBase(int BindingIndex)
    {
        GL.BindBufferBase((BufferRangeTarget)Target, BindingIndex, BufferID);
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
            CollectionInternal.Clear();
        }
    }
}


