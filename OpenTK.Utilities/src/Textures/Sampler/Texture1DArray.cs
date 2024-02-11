using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class Texture1DArray() : TexturesImplements(TextureTarget2d.Texture1DArray), ITexture2D
{
    public Texture1DArray(SizedInternalFormat SizedInternalFormat, int width, int layers, int levels = 1)
        : this()
    {
        this.ToAllocate(SizedInternalFormat, width, layers, levels);
    }

    public new int Width => base.Width;

    public new int Height => base.Height;

    public Size Size => new Size(base.Width, base.Height);

    public Texture2DWrapping Wrapping
    {
        get => new (this.WrapModeS, this.WrapModeT);
        set
        {
            this.SetWrapModeS(value.WrapModeS);
            this.SetWrapModeT(value.WrapModeT);
        }
    }

    public Vector2i GetSizeMipmap(int level = 0)
    {
        this.GetSizeMipmap(out var width, out var layer, out _, level);
        return new Vector2i(width, layer);
    }

    public void GetSizeMipmap(out int width, out int layer, int level = 0)
    {
        this.GetSizeMipmap(out width, out layer, out _, level);
    }

    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int layers, int levels = 1)
    {
        this.NewAllocation(SizedInternalFormat, width, layers, 1, levels);
    }

    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public unsafe void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        this.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width, int layers,
        int srcLevel, int srcX, int srcLayer,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : ITexture
    {
        return this.CopySubData(
            dstTexture,
            width, layers, 1,
            srcLevel, srcX, srcLayer, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture2D CloneGPU()
    {
        Texture2D dstTexture = new Texture2D(this.InternalFormat, base.Width, base.Height, this.Levels);

        for (int level = 0; level < this.Levels; level++)
        {
            this.CopySubData(
                dstTexture,
                base.Width, base.Height, 1,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}
