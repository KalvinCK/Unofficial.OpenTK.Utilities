using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface IReadOnlyTexture : IReadOnlyBuffer
{
    public TextureDimension Dimension { get; }

    public TextureTarget Target { get; }

    public TextureFormat Format { get; }

    public bool HasAllocated { get; }

    /// <summary>
    /// Gets handler of this texture,
    /// Gets handler of this texture, makes use of the extension: <c>GL_ARB_bindless_texture</c>.
    /// </summary>
    /// <remarks>
    /// For its operation after declaring <c>'#version --- </c>' use:
    /// <para>
    /// <c>#extension GL_ARB_bindless_texture : required</c>.
    /// </para>
    /// </remarks>
    public long BindlessHandler { get; }

    public void ClearContext();

    public void Bind();

    public void BindToUnit(int unit);

    public void BindToImageUnit(int Unit, int level, bool layered, int layer, TextureAccess TextureAccess);

    public static void ClearContext(TextureTarget TextureTarget)
    {
        GL.BindTexture(TextureTarget, 0);
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

    internal static int CreateTextureBuffer(TextureTarget TextureTarget)
    {
        GL.CreateTextures(TextureTarget, 1, out int buffer);
        return buffer;
    }
}
