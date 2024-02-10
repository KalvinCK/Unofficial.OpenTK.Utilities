using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class BufferConstant<T> : 
    IBufferObject, IDisposable where T : struct
{
    public BufferTarget Target { get; }
    public int BufferID { get; } = IBufferObject.CreateBuffer();
    public int Stride { get; } = Unsafe.SizeOf<T>();
    public int Count { get; } = 1;
    public int MemorySize => Count * Stride;

    public unsafe BufferConstant(BufferTarget bufferTarget)
    {
        Target = bufferTarget;
        GL.NamedBufferStorage(BufferID, MemorySize, IntPtr.Zero, BufferStorageFlags.DynamicStorageBit);
        _Data = new();
    }
    public unsafe BufferConstant(BufferTarget bufferTarget, T InitData)
    {
        Target = bufferTarget;
        GL.NamedBufferStorage(BufferID, MemorySize, ref InitData, BufferStorageFlags.DynamicStorageBit);
        _Data = InitData;
    }
    public void Bind(BufferTarget BufferTarget)
    {
        GL.BindBuffer(BufferTarget, BufferID);
    }
    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int BindingIndex)
    {
        GL.BindBufferBase(BufferRangeTarget, BindingIndex, BufferID);
    }
    private void UpdateRegion<TValue>(int offsetInBytes, int sizeInBytes, TValue Data) where TValue : struct
    {
        GL.NamedBufferSubData(BufferID, offsetInBytes, sizeInBytes, ref Data);
    }

    private T _Data;
    public T Data
    {
        get => _Data;
        set
        {
            _Data = value;
            UpdateRegion(0, MemorySize, Data);
        }
    }

    /// <summary>
    /// UpdatePixels the property of: <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of property to update.</typeparam>
    /// <param name="propName">The name of the property.</param>
    /// <param name="value"></param>
    public virtual unsafe void UpdatePropData<TValue>(string propName, in TValue value) where TValue : unmanaged
    {
        int offset = Marshal.OffsetOf(typeof(T), propName).ToInt32();

        fixed (void* ptrData = &_Data)
        {
            void* ptrField = (byte*)ptrData + offset;
            Marshal.StructureToPtr(value, (IntPtr)ptrField, false);

            UpdateRegion(offset, Unsafe.SizeOf<TValue>(), value);
        }
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
        }
    }
}