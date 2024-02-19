using System.Collections;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IBufferObject, IDisposable
    where TKey : unmanaged
    where TValue : struct
{
    private Dictionary<TKey, TValue> keyValueDatas = [];

    public BufferUsageHint UsageHint { get; } = BufferUsageHint.StreamDraw;

    public int BufferID { get; } = IBufferObject.CreateBuffer();

    public int Stride { get; } = Unsafe.SizeOf<TValue>();

    public int Count => this.keyValueDatas.Count;

    public int MemorySize => this.Count * this.Stride;

    public TValue this[TKey key]
    {
        get => this.keyValueDatas[key];
        set
        {
            TValue newData = value;
            this.keyValueDatas[key] = newData;
            this.UpdateCollectionBuffer();
        }
    }

    public void Add(TKey key, TValue value)
    {
        this.keyValueDatas.Add(key, value);
        this.UpdateCollectionBuffer();
    }

    public bool Remove(TKey key)
    {
        var result = this.keyValueDatas.Remove(key);
        this.UpdateCollectionBuffer();
        return result;
    }

    public void Clear()
    {
        this.Clear();
        GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, IntPtr.Zero);
    }

    public bool Contains(KeyValuePair<TKey, TValue> value)
    {
        return this.keyValueDatas.Contains(value);
    }

    public bool ContainsKey(TKey key)
    {
        return this.keyValueDatas.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        return this.keyValueDatas.ContainsValue(value);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return this.keyValueDatas.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.keyValueDatas.GetEnumerator();
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
            this.keyValueDatas = [];
        }
    }

    private void UpdateCollectionBuffer()
    {
        if (this.Count > 0)
        {
            var datas = this.keyValueDatas.Values.ToArray();
            GL.NamedBufferData(this.BufferID, this.MemorySize, datas, this.UsageHint);
        }
    }
}
