using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferFrame<T> : IBufferObject, IDisposable
    where T : struct
{
    private readonly BufferUsageHint usageHint = BufferUsageHint.DynamicDraw;

    private T[] memoryData = [];

    public BufferFrame(int countMemoryElements)
    {
        this.SetCapacity(countMemoryElements);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public int Count { get; protected set; }

    public int MemorySize => this.Count * this.Stride;

    public int CountCurrentFrame { get; private set; }

    private int EndIndex => this.memoryData.Length - 1;

    public T this[int index] => this.memoryData[index];

    public void SetCapacity(int countMemoryElements)
    {
        this.Count = countMemoryElements;

        Array.Resize(ref this.memoryData, countMemoryElements);
        GL.NamedBufferData(this.BufferID, this.MemorySize, this.memoryData, this.usageHint);
    }

    public bool Include(T Data)
    {
        if (this.CountCurrentFrame == this.EndIndex)
        {
            return false;
        }

        this.memoryData[this.CountCurrentFrame] = Data;
        GL.NamedBufferSubData(this.BufferID, this.CountCurrentFrame * this.Stride, this.Stride, ref Data);

        this.CountCurrentFrame++;

        return true;
    }

    public void NewFrame()
    {
        if (this.CountCurrentFrame == 0)
        {
            return;
        }

        if (this.CountCurrentFrame == this.EndIndex)
        {
            this.SetCapacity((int)(this.Count * 1.5f));
            Debug.Print("The extended buffer size.");
        }
        else
        {
            GL.NamedBufferSubData(this.BufferID, 0, this.MemorySize, IntPtr.Zero);
        }

        Array.Clear(this.memoryData);
        this.CountCurrentFrame = 0;
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
        return $"Stride: [{this.Stride}] Count of elements: [{this.Count}] Total capacity in bytes: [{this.MemorySize}] Count of elements in Frame: [{this.CountCurrentFrame}]\n";
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
            this.CountCurrentFrame = 0;
            this.memoryData = [];
        }
    }
}
