using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface ISamplerObject : IBuffer
{
    public long BindlessHandler { get; }

    internal static int CreateBuffer()
    {
        GL.CreateSamplers(1, out int buffer);
        return buffer;
    }

    public static void ClearContext(int unit)
    {
        GL.BindSampler(unit, 0);
    }

    public static void MultiBind(int first, int[] units)
    {
        GL.BindSamplers(first, units.Length, units);
    }
}
