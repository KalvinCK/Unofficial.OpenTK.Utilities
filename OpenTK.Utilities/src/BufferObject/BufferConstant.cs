using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferConstant<T> : IBufferObject, IDisposable
    where T : struct
{
    private T data;

    public unsafe BufferConstant()
    {
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, IntPtr.Zero, BufferStorageFlags.DynamicStorageBit);
        this.data = new ();
    }

    public unsafe BufferConstant(T InitData)
    {
        GL.NamedBufferStorage(this.BufferID, this.MemorySize, ref InitData, BufferStorageFlags.DynamicStorageBit);
        this.data = InitData;
    }

    public T Data
    {
        get => this.data;
        set
        {
            this.data = value;
            this.UpdateBufferData(0, this.MemorySize, this.data);
        }
    }

    public int BufferID { get; } = IBufferObject.CreateBuffer();

    public int Stride { get; } = Unsafe.SizeOf<T>();

    public int Count { get; } = 1;

    public int MemorySize => this.Count * this.Stride;

    public virtual unsafe void UpdateBufferDataPropData<TValue>(string propName, in TValue value)
        where TValue : unmanaged
    {
        int offset = Marshal.OffsetOf(typeof(T), propName).ToInt32();

        fixed (void* ptrData = &this.data)
        {
            void* ptrField = (byte*)ptrData + offset;
            Marshal.StructureToPtr(value, (IntPtr)ptrField, false);

            this.UpdateBufferData(offset, Unsafe.SizeOf<TValue>(), value);
        }
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
        }
    }

    private void UpdateBufferData<TValue>(int offsetInBytes, int sizeInBytes, TValue data)
        where TValue : struct
    {
        GL.NamedBufferSubData(this.BufferID, offsetInBytes, sizeInBytes, ref data);
    }
}
