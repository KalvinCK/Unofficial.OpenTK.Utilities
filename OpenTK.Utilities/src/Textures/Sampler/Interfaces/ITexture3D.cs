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

    public TextureFiltering Filtering { get; set; }

    public Texture3DWrapping Wrapping { get; set; }

    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0);
}
