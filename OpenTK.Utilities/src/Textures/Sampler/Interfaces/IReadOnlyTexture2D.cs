using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public interface IReadOnlyTexture2D : IReadOnlyTexture
{
    public int Levels { get; }

    public int Width { get; }

    public int Height { get; }

    public Size Size { get; }

    public TextureFiltering Filtering { get; set; }

    public Texture2DWrapping Wrapping { get; set; }

    public void GetImageData(PixelFormat PixelFormat, PixelType PixelType, IntPtr pixels, int bufSize, int level = 0);
}
