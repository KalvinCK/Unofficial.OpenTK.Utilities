using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferPassFrame<T> : IBufferObject, IDisposable
    where T : struct
{
    private readonly BufferUsageHint usageHint = BufferUsageHint.StreamDraw;

    public BufferPassFrame(int initialCount = 28)
    {
        if (initialCount < 1)
        {
            throw new EmptyAllocationException();
        }

        this.CountBuffer = initialCount;
        GL.NamedBufferData(this.BufferID, this.MemorySize, IntPtr.Zero, this.usageHint);

        this.Reserve(initialCount);
    }

    #region Props
    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    /// <summary>
    /// Gets the number of elements in the current frame pass.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets the number of elements relative to the buffer.
    /// </summary>
    public int CountBuffer { get; private set; }

    public int MemorySize => this.CountBuffer * this.Stride;

    #endregion
    public void Include(T data)
    {
        if (this.Count == this.CountBuffer)
        {
            T[] currentDatas = new T[this.CountBuffer];
            GL.GetNamedBufferSubData(this.BufferID, 0, this.MemorySize, currentDatas);

            this.CountBuffer *= 2;
            GL.NamedBufferData(this.BufferID, this.MemorySize, currentDatas, this.usageHint);
            currentDatas = [];
        }

        GL.NamedBufferSubData(this.BufferID, this.Count * this.Stride, this.Stride, ref data);
        this.Count++;
    }

    public void Clear()
    {
        GL.NamedBufferSubData(this.BufferID, 0, this.Count * this.Stride, IntPtr.Zero);
    }

    #region ###
    public IEnumerator<T> GetEnumerator()
    {
        T[] currentDatas = new T[this.Count];
        GL.GetNamedBufferSubData(this.BufferID, 0, this.Count * this.Stride, currentDatas);
        foreach (T data in currentDatas)
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
        return $"Stride: [{this.Stride}] Count in current Frame: [{this.Count}] CountBuffer: [{this.CountBuffer}] allocated memory size: [{this.MemorySize}]";
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
        }
    }
    #endregion

    private void ValidateIndex(int index)
    {
        if ((index >= 0 && index <= this.Count - 1) is false)
        {
            throw new IndexOutOfRangeException("Index was outside the bounds of the buffer.");
        }
    }

    private T[] GetBufferDatas()
    {
        T[] data = new T[this.Count];
        GL.GetNamedBufferSubData(this.BufferID, 0, this.MemorySize, data);
        return data;
    }

    private void Reserve(int count)
    {
        this.Count = count;
        GL.NamedBufferData(this.BufferID, this.MemorySize, IntPtr.Zero, this.usageHint);
    }
}
