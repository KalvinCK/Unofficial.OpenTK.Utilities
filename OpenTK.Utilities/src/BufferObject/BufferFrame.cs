using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferFrame<T> :
    IBufferObject, IDisposable where T : struct
{
    public BufferTarget Target { get; }
    public BufferUsageHint UsageHint { get; } = BufferUsageHint.DynamicDraw;
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public int Count { get; protected set; }
    public int MemorySize => Count * Stride;
    public int CountCurrentFrame { get; private set; }

    private T[] MemoryData = [];
    private int EndIndex => MemoryData.Length - 1;
    public T this[int index] => MemoryData[index];
    public BufferFrame(BufferTarget BufferTarget, int countMemoryElements)
    {
        Target = BufferTarget;
        SetCapacity(countMemoryElements);
    }
    public void SetCapacity(int countMemoryElements)
    {
        Count = countMemoryElements;

        Array.Resize(ref MemoryData, countMemoryElements);
        GL.NamedBufferData(BufferID, MemorySize, MemoryData, UsageHint);
    }
    public bool Include(T Data)
    {
        if(CountCurrentFrame == EndIndex)
        {
            return false;
        }

        MemoryData[CountCurrentFrame] = Data;
        GL.NamedBufferSubData(BufferID, CountCurrentFrame * Stride, Stride, ref Data);

        CountCurrentFrame++;

        return true;
    }
    public void NewFrame()
    {
        if(CountCurrentFrame == 0)
            return;

        if(CountCurrentFrame == EndIndex)
        {
            SetCapacity((int)(Count * 1.5f));
            Debug.Print("The extended buffer size.");
        }
        else
        {
            GL.NamedBufferSubData(BufferID, 0, MemorySize, IntPtr.Zero);
        }

        Array.Clear(MemoryData);
        CountCurrentFrame = 0;
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
        return $"Target: [{Target}] Stride: [{Stride}] Count of elements: [{Count}] Total capacity in bytes: [{MemorySize}] Count of elements in Frame: [{CountCurrentFrame}]\n";
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
            CountCurrentFrame = 0;
            MemoryData = [];
        }
    }
}


