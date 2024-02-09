using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface ITexture1D : ITexture
{
    public int Levels { get; }
    public int Width { get; }
    public TextureMinFilter MinFilter { get; }
    public TextureMagFilter MagFilter { get; }
    public TextureWrapMode WrapModeS { get; }
    public void SetFiltering(TextureFiltering filtering);
    public void SetFiltering(TextureMinFilter minFilter, TextureMagFilter magFilter);
    public void SetWrapping(TextureWrapMode wrapS);
    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0);
}
