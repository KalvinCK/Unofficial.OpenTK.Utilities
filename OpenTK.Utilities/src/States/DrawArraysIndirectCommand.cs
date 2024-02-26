using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

/// <summary>
/// Used for buffers intended for '<see cref="BufferTarget.DrawIndirectBuffer"/>'.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct DrawArraysIndirectCommand
{
    public static readonly int Stride = Unsafe.SizeOf<DrawArraysIndirectCommand>();

    public int Count;
    public int InstanceCount;
    public int Base;
    public int BaseInstance;
}
