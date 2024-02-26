﻿using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.BufferObjects;

public interface IReadOnlyBufferObject : IReadOnlyBuffer
{
    /// <summary>
    /// Gets The Size of the internal type in bytes.
    /// </summary>
    public int Stride { get; }

    /// <summary>
    /// Gets Count of elements allocated to the buffer.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets Memory size in bytes.
    /// </summary>
    public int MemorySize { get; }

    /// <summary>
    /// Bind buffer in context.
    /// </summary>
    /// <param name="BufferTarget">PxType of target to be linked.</param>
    public void Bind(BufferTarget BufferTarget);

    /// <summary>
    /// Binds the buffer to an index.
    /// </summary>
    /// <param name="BufferRangeTarget">Buffer type.</param>
    /// <param name="bindingIndex">Index block.</param>
    public void BindBufferBase(BufferRangeTarget BufferRangeTarget, int bindingIndex);

    internal static int CreateBuffer()
    {
        GL.CreateBuffers(1, out int buffer);
        return buffer;
    }
}