using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities;

public static partial class Context
{
    public static ErrorCode Error => GL.GetError();

    internal static unsafe void GetVec2i(GetPName GetPName, ref Vector2i value)
    {
        fixed (int* ptr = &value.X)
        {
            GL.GetInteger(GetPName, ptr);
        }
    }

    internal static unsafe void GetVec2(GetPName GetPName, ref Vector2 value)
    {
        fixed (float* ptr = &value.X)
        {
            GL.GetFloat(GetPName, ptr);
        }
    }

    internal static unsafe void GetVec3i(GetPName GetPName, ref Vector3i value)
    {
        fixed (int* ptr = &value.X)
        {
            GL.GetInteger(GetPName, ptr);
        }
    }

    internal static unsafe void GetVec3(GetPName GetPName, ref Vector3 value)
    {
        fixed (float* ptr = &value.X)
        {
            GL.GetFloat(GetPName, ptr);
        }
    }
}
