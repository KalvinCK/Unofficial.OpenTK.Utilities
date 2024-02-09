using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface ITexture : IBuffer
{
    public TextureDimension Dimension { get; }
    public TextureTarget Target { get; }
    public SizedInternalFormat InternalFormat { get; }
    public bool HasAllocated { get; }
    public long BindlessHandler { get; }

    public void ClearContext();
    public void Bind();
    public void BindToUnit(int unit);
    public void BindToImageUnit(int Unit, int level, bool layered, int layer, TextureAccess textureAccess);

    public static void ClearContext(TextureTarget target)
    {
        GL.BindTexture(target, 0);
    }
    public static int GetMaxMipmapLevel(int width, int height, int depth)
    {
        return MathF.ILogB(Math.Max(width, Math.Max(height, depth))) + 1;
    }
    public static unsafe void MultiBindToUnit(int first, int length, int* textures)
    {
        GL.BindTextures(first, length, textures);
    }
    public static void MultiBindToUnit(int first, int[] textures)
    {
        GL.BindTextures(first, textures.Length, textures);
    }

    internal static int CreateTextureBuffer(TextureTarget target)
    {
        GL.CreateTextures(target, 1, out int buffer);
        return buffer;
    }
}
