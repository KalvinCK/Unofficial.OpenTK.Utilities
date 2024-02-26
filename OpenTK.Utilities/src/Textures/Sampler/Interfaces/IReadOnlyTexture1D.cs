using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface IReadOnlyTexture1D : IReadOnlyTexture
{
    public int Levels { get; }

    public int Width { get; }

    public TextureFiltering Filtering { get; set; }

    public TextureWrapMode WrapModeS { get; set; }

    public void GetImageData(PixelFormat PixelFormat, PixelType PixelType, IntPtr pixels, int bufSize, int level = 0);
}
