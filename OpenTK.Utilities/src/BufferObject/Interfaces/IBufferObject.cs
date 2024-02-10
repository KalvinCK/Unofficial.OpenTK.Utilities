using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public interface IBufferObject : IBuffer
{
    /// <summary>
    /// The Size of the internal type TTexture in bytes.
    /// </summary>
    public int Stride { get; }

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
    public void Bind(BufferTarget BufferTarget);

    /// <summary>
    /// Link to an index.
    /// </summary>
    /// <param name="BufferRangeTarget">Buffer type.</param>
    /// <param name="bindingIndex"></param>
    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int bindingIndex);

    /// <summary>
    /// Clears the context for this type of buffer.
    /// </summary>
    public static void ClearContext(BufferTarget BufferTarget)
    {
        GL.BindBuffer(BufferTarget, 0);
    }

    internal static int CreateBuffer()
    {
        GL.CreateBuffers(1, out int buffer); return buffer;
    }
    internal static string Unnamed = "UNNAMED";
}