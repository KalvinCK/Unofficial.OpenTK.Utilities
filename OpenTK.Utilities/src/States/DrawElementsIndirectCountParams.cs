using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

#pragma warning disable SA1202 // Elements should be ordered by access
/// <summary>
/// Used for buffers intended for '<see cref="BufferTarget.ParameterBuffer"/>'.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct DrawElementsIndirectCountParams
{
    public static readonly int Stride = Unsafe.SizeOf<DrawElementsIndirectCountParams>();

    private readonly int pad0;
    public int InstanceCount;
    private readonly int pad1;
    private readonly int pad2;
    public int BaseInstance;
}
#pragma warning restore SA1202 // Elements should be ordered by access

