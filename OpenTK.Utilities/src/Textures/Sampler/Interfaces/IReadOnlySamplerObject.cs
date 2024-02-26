using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface IReadOnlySamplerObject : IReadOnlyBuffer
{
    public long BindlessHandler { get; }

    public void Bind(int unit);

    public static void MultiBind(int first, int[] units)
    {
        GL.BindSamplers(first, units.Length, units);
    }

    internal static int CreateBuffer()
    {
        GL.CreateSamplers(1, out int buffer);
        return buffer;
    }
}
