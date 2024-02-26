using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class Texture2D() : TexturesImplements(TextureTarget2d.Texture2D), IReadOnlyTexture2D
{
    public Texture2D(TextureFormat TextureFormat, int width, int height, int levels = 1)
        : this()
    {
        this.AllocateStorage(TextureFormat, width, height, levels);
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

    public void AllocateStorage(TextureFormat TextureFormat, int width, int height, int levels = 1)
    {
        this.Storage(TextureFormat, width, height, 1, levels);
    }

    #region
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public unsafe void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public void Update(int width, int height, PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        this.UpdateSubData(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    #endregion

    #region UpdateCompress
    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }

    public void UpdateCompress<T>(int width, int height, PixelFormat PixelFormat, int imageSize, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }

    public void UpdateCompress(int width, int height, PixelFormat PixelFormat, int imageSize, nuint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        this.UpdateSubDataCompress(TextureDimension.Two, width, height, 1, PixelFormat, imageSize, pixels, level, xOffset, yOffset, 0);
    }
    #endregion

    public override string ToString()
    {
        return
            $"Dimension: {this.Dimension}\n" +
            $"Target: {this.Target}\n" +
            $"InternalFormat: {this.Format}\n" +
            $"HasAllocated: {this.HasAllocated}\n" +
            $"Levels: {this.Levels}\n" +
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
        where TTexture : IReadOnlyTexture
    {
        return this.CopySubData(
            dstTexture,
            width, height, 1,
            srcLevel, srcX, srcY, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture2D CloneGPU()
    {
        Texture2D dstTexture = new Texture2D(this.Format, base.Width, base.Height, this.Levels);

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
