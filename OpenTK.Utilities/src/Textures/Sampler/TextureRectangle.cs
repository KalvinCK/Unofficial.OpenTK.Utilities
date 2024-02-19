using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class TextureRectangle : TexturesImplements, ITexture2D
{
    public TextureRectangle()
        : base(TextureTarget2d.TextureRectangle)
    {
        this.TextureFiltering = new TextureFiltering(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        this.WrapModeS = this.WrapModeT = TextureWrapMode.ClampToEdge;
    }

    public TextureRectangle(TextureFormat TextureFormat, int width, int height)
        : this()
    {
        this.ToAllocate(TextureFormat, width, height);
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
                Helpers.Print("Rectangle texture do not support mipmaps filtering.");
            }
        }
    }

    public new void GenerateMipmap()
    {
        Helpers.Print("Filters related to mipmaps cannot be created applied to a 'TextureRectangle'.");
    }

    public Vector2i GetSizeMipmap()
    {
        this.GetSizeMipmap(out var width, out var layer, out _, 0);
        return new Vector2i(width, layer);
    }

    public void GetSizeMipmap(out int width, out int layer)
    {
        this.GetSizeMipmap(out width, out layer, out _, 0);
    }

    public void ToAllocate(TextureFormat TextureFormat, int width, int height)
    {
        this.Storage(TextureFormat, width, height, 1, 1);
    }

    #region Update
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, ReadOnlySpan<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }

    public unsafe void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }

    public void Update(int width, int height, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int xOffset = 0, int yOffset = 0)
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, 0, xOffset, yOffset);
    }
    #endregion

    #region UpdateCompress
    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, List<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, Span<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, ReadOnlySpan<T> pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, T[] pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, in T pixels, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }

    public void UpdateCompress(int width, int height, PixelFormat PixelFormat, int imageSize, nuint pixels, int xOffset = 0, int yOffset = 0)
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, 0, xOffset, yOffset, 0);
    }
    #endregion

    public override string ToString()
    {
        return
            $"Dimension: {this.Dimension}\n" +
            $"Target: {this.Target}\n" +
            $"InternalFormat: {this.Format}\n" +
            $"HasAllocated: {this.HasAllocated}\n" +
            $"Width: {this.Width}\n" +
            $"Height: {this.Height}\n" +
            $"Size: {this.Size}\n" +
            $"Filtering: {this.Filtering}\n" +
            $"Wrapping: {this.Wrapping}\n";
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
        TextureRectangle dstTexture = new TextureRectangle(this.Format, base.Width, base.Height);

        this.CopySubData(
            dstTexture,
            base.Width, base.Height, 1,
            0, 0, 0, 0,
            0, 0, 0, 0);

        return dstTexture;
    }
}
