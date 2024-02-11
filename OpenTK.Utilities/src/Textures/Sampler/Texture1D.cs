using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class Texture1D() : TexturesImplements(TextureTarget1d.Texture1D), ITexture1D
{
    public Texture1D(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1)
        : this()
    {
        this.ToAllocate(SizedInternalFormat, width, height, levels);
    }

    public new int Width => base.Width;

    public new TextureWrapMode WrapModeS
    {
        get => this.WrapModeS;
        set => this.SetWrapModeS(value);
    }

    public int GetSizeMipmap(int level = 0)
    {
        this.GetSizeMipmap(out int width, out _, out _, level);
        return width;
    }

    public void GetSizeMipmap(out int width, int level = 0)
    {
        this.GetSizeMipmap(out width, out _, out _, level);
    }

    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1)
    {
        this.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }

    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, List<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, Span<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, T[] pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public unsafe void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, in T pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update(int width, PixelFormat PixelFormat, PixelType PixelType, IntPtr pixels, int level = 0, int xOffset = 0)
    {
        this.UpdatePixels(TextureDimension.One, width, 0, 0, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width,
        int srcLevel, int srcX,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : ITexture
    {
        return this.CopySubData(
            dstTexture,
            width, 1, 1,
            srcLevel, srcX, 0, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture1D CloneGPU()
    {
        Texture1D dstTexture = new Texture1D(this.InternalFormat, base.Width, this.Levels);

        for (int level = 0; level < this.Levels; level++)
        {
            this.CopySubData(
                dstTexture,
                base.Width, 1, 1,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}
