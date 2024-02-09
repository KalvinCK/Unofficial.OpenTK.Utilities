using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public interface IBufferObject : IBuffer
{
    /// <summary>
    /// The Size of the internal type TTexture in bytes.
    /// </summary>
    public int Stride { get; }

    /// <summary>
    /// Target denominated for buffer.
    /// </summary>
    public BufferTarget Target { get; }

    /// <summary>
    /// Count of elements allocated to the buffer.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Memory size in bytes.
    /// </summary>
    public int MemorySize { get; }

    /// <summary>
    /// Bind buffer in context.
    /// </summary>
    public void Bind();

    /// <summary>
    /// Link to an index.
    /// </summary>
    /// <param name="BufferRangeTarget">Buffer type.</param>
    /// <param name="bindingIndex"></param>
    public void BindBufferBase(int bindingIndex);

    /// <summary>
    /// Clears the context for this type of buffer.
    /// </summary>
    public void ClearContext();

    internal static int CreateBuffer()
    {
        GL.CreateBuffers(1, out int buffer); return buffer;
    }
}