using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

/// <summary>
/// Used for buffers intended for '<see cref="BufferTarget.DrawIndirectBuffer"/>'.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct DrawElementsIndirectCommand
{
    public static readonly int Stride = Unsafe.SizeOf<DrawElementsIndirectCommand>();

    public int Count;
    public int InstanceCount;
    public int Base;
    public int BaseVertex;
    public int BaseInstance;
}
