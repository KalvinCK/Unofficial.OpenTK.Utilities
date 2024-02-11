using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class TextureRectangle : TexturesImplements, ITexture2D
{
    public TextureRectangle()
        : base(TextureTarget2d.TextureRectangle)
    {
        this.TextureFiltering = new TextureFiltering(false, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        this.WrapModeS = this.WrapModeT = TextureWrapMode.ClampToEdge;
    }

    public TextureRectangle(SizedInternalFormat SizedInternalFormat, int width, int height)
        : this()
    {
        this.ToAllocate(SizedInternalFormat, width, height);
    }

    public new int Width => base.Width;

    public new int Height => base.Height;

    public Size Size => new Size(base.Width, base.Height);

    public new TextureFiltering Filtering
    {
        get => base.Filtering;
        set
        {
            var filters = new[] { 9728, 9729 };
            var resultMin = filters.Contains((int)value.MinFilter);
            var resultMag = filters.Contains((int)value.MagFilter);

            if (resultMin && resultMag)
            {
                base.Filtering = value;
            }
            else
            {
                Helpers.PrintWarning("Rectangle texture do not support mipmaps filtering.");
            }
        }
    }

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

    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height)
    {
        this.NewAllocation(SizedInternalFormat, width, height, 1, 1);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public unsafe void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update(int width, int height, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        this.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width, int height,
        int srcLevel, int srcX, int srcY,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : ITexture
    {
        return this.CopySubData(
            dstTexture,
            width, height, 1,
            srcLevel, srcX, srcY, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public TextureRectangle CloneGPU()
    {
        TextureRectangle dstTexture = new TextureRectangle(this.InternalFormat, base.Width, base.Height);

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
