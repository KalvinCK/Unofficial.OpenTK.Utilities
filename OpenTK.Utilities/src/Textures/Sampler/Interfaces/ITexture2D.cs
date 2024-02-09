using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public interface ITexture2D : ITexture
{
    public int Levels { get; }
    public int Width { get; }
    public int Height { get; }
    public Size Size { get; }
    public TextureMinFilter MinFilter { get; }
    public TextureMagFilter MagFilter { get; }
    public TextureWrapMode WrapModeS { get; }
    public TextureWrapMode WrapModeT { get; }
    public void SetFiltering(TextureFiltering filtering);
    public void SetFiltering(TextureMinFilter minFilter, TextureMagFilter magFilter);
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT);
    public void SetWrapping(Texture2DWrapping wrapping);
    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0);
}
