using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class Texture1D() : TexturesImplements(TextureTarget1d.Texture1D), ITexture1D
{
    public int Levels => base._Levels;
    public int Width => base._Width;
    public TextureMinFilter MinFilter => base._MinFilter;
    public TextureMagFilter MagFilter => base._MagFilter;
    public TextureWrapMode WrapModeS => base._WrapModeS;
    public Texture1D(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1) : this()
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1)
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    public int GetSizeMipmap(int level = 0)
    {
        base.GetSizeMipmap(out int width, out _, out _, level);
        return width;
    }
    public void GetSizeMipmap(out int width, int level = 0)
    {
        base.GetSizeMipmap(out width, out _, out _, level);
    }
    public void SetWrapping(TextureWrapMode wrapS)
    {
        base.SetWrapMode(wrapS);
    }
    public void Update<T>(int width, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.One, width, 1, 1, pixelFormat, pixelType, pixels, level, xOffset);
    }
    public void Update<T>(int width, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.One, width, 1, 1, pixelFormat, pixelType, pixels, level, xOffset);
    }
    public void Update<T>(int width, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.One, width, 1, 1, pixelFormat, pixelType, pixels, level, xOffset);
    }
    public unsafe void Update<T>(int width, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.One, width, 1, 1, pixelFormat, pixelType, pixels, level, xOffset);
    }
    public void Update(int width, PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int level = 0, int xOffset = 0)
    {
        base.UpdatePixels(TextureDimension.One, width, 0, 0, pixelFormat, pixelType, pixels, level, xOffset);
    }

    public bool CopyGPU<TTexture>(TTexture dstTexture,
        int width,
        int srcLevel, int srcX,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        return base.CopySubData(dstTexture,
            width, 1, 1,
            srcLevel, srcX, 0, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture1D CloneGPU(bool clone_Wrap_Filters = true)
    {
        Texture1D dstTexture = new Texture1D(_InternalFormat, _Width, _Levels);

        if (clone_Wrap_Filters)
        {
            base.CopyFilters(dstTexture);
            base.CopyWrapModes(dstTexture);
        }

        for (int level = 0; level < _Levels; level++)
        {
            base.CopySubData(dstTexture,
                Width, 1, 1,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}

