using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public class BufferConstant<T> : IBufferObject, IDisposable
    where T : struct
{
    private const BufferAccessMask BufferFlags = BufferAccessMask.MapReadBit | BufferAccessMask.MapWriteBit | BufferAccessMask.MapPersistentBit | BufferAccessMask.MapCoherentBit;

    public BufferConstant(T initData = default)
    {
        GL.NamedBufferStorage(this.BufferID, this.Stride, ref initData, (BufferStorageFlags)BufferFlags);
        this.PtrRegion = GL.MapNamedBufferRange(this.BufferID, 0, this.Stride, BufferFlags);
    }

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int BufferID { get; private set; } = IBufferObject.CreateBuffer();

    public int MemorySize { get; private set; } = Unsafe.SizeOf<T>();

    public int Count { get; private set; } = 1;

    public unsafe ref T Data => ref Unsafe.AsRef<T>(this.PtrRegion.ToPointer());

    private IntPtr PtrRegion { get; set; }

    public void ForceSync()
    {
        GL.FlushMappedNamedBufferRange(this.BufferID, 0, this.MemorySize);
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
            this.MemorySize = 0;
        }
    }
}
