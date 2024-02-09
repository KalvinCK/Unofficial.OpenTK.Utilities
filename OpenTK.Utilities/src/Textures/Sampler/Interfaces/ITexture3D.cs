using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public interface ITexture3D : ITexture
{
    public int Levels { get; }
    public int Width { get; }
    public int Height { get; }
    public int Layers { get; }
    public Vector3i Size { get; }
    public TextureMinFilter MinFilter { get; }
    public TextureMagFilter MagFilter { get; }
    public TextureWrapMode WrapModeS { get; }
    public TextureWrapMode WrapModeT { get; }
    public TextureWrapMode WrapModeR { get; }
    public void SetFiltering(TextureFiltering filtering);
    public void SetFiltering(TextureMinFilter minFilter, TextureMagFilter magFilter);
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT, TextureWrapMode wrapR);
    public void SetWrapping(Texture3DWrapping wrapping);
    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0);
}
