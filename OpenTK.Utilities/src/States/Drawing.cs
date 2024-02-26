using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Utilities.BufferObjects;
using OpenTK.Utilities.Objects;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1201 // Elements should appear in the correct order

/// <summary>
/// Meticulously manages essential states for the OpenGL rendering pipeline,
/// with a variety of rasterization-related states to control crucial aspects of
/// scene rendering, offering fine-grained control over the rendering flow.
/// </summary>
public static class Drawing
{
    private static PrimitiveType mode = PrimitiveType.Triangles;

    public static PrimitiveType Primitive
    {
        get => mode;
        set => mode = value;
    }

    public static void Draw(int vertsCount, int instanceCount = 1, int baseVert = 0, int baseInstance = 0)
    {
        GL.DrawArraysInstancedBaseInstance(mode, baseVert, vertsCount, instanceCount, baseInstance);
    }

    public static void DrawElements(DrawElementsType type, int elementsCount, int instanceCount = 1, int baseElement = 0, int baseInstance = 0)
    {
        GL.DrawElementsInstancedBaseVertexBaseInstance(mode, elementsCount, type, IntPtr.Zero, instanceCount, baseElement, baseInstance);
    }

    public static void DrawIndirect<TBufferObj>(TBufferObj bufferObject)
        where TBufferObj : IReadOnlyBufferObject
    {
        bufferObject.Bind(BufferTarget.DrawIndirectBuffer);
        GL.DrawArraysIndirect(mode, IntPtr.Zero);
    }

    public static void DrawElementsIndirect<TBufferObj>(TBufferObj bufferObject, DrawElementsType type)
        where TBufferObj : IReadOnlyBufferObject
    {
        bufferObject.Bind(BufferTarget.DrawIndirectBuffer);
        GL.DrawElementsIndirect(mode, type, IntPtr.Zero);
    }

    public static void MultiDrawArrays<TBufferObj>(TBufferObj bufferObject, int drawCount = 1)
        where TBufferObj : IReadOnlyBufferObject
    {
        bufferObject.Bind(BufferTarget.DrawIndirectBuffer);
        GL.MultiDrawArraysIndirect(mode, IntPtr.Zero, drawCount, bufferObject.Stride);
    }

    public static void MultiDrawElements<TBufferObj>(TBufferObj bufferObject, DrawElementsType type, int drawCount = 1)
        where TBufferObj : IReadOnlyBufferObject
    {
        bufferObject.Bind(BufferTarget.DrawIndirectBuffer);
        GL.MultiDrawElementsIndirect(mode, type, IntPtr.Zero, drawCount, bufferObject.Stride);
    }

    public static void MultiDrawArraysCount(
        IReadOnlyBufferObject bufferDrawIndirect,
        IReadOnlyBufferObject bufferDrawParameter,
        int drawCount = 1, int maxDrawCount = 1)
    {
        bufferDrawIndirect.Bind(BufferTarget.DrawIndirectBuffer);
        bufferDrawParameter.Bind(BufferTarget.ParameterBuffer);

        GL.MultiDrawArraysIndirectCount(mode, IntPtr.Zero, drawCount * 4, maxDrawCount, DrawElementsIndirectCountParams.Stride);
    }

    public static void MultiDrawElementsCount(
        IReadOnlyBufferObject bufferDrawIndirect,
        IReadOnlyBufferObject bufferDrawParameter,
        DrawElementsType type, int drawCount = 1, int maxDrawCount = 1)
    {
        bufferDrawIndirect.Bind(BufferTarget.DrawIndirectBuffer);
        bufferDrawParameter.Bind(BufferTarget.ParameterBuffer);

        GL.MultiDrawElementsIndirectCount(mode, type, IntPtr.Zero, drawCount * 4, maxDrawCount, DrawElementsIndirectCountParams.Stride);
    }

    /// <summary>
    /// Reset the context of the following objects
    /// <see cref="VertexArrayObject"/>, <see cref="ShaderObject"/> and <see cref="PipelineObject"/>.
    /// </summary>
    public static void ResetDrawingContext()
    {
        GL.BindVertexArray(0);
        GL.BindProgramPipeline(0);
        GL.UseProgram(0);
    }
}
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
