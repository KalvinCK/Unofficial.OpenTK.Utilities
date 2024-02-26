using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class Texture1D() : TexturesImplements(TextureTarget1d.Texture1D), IReadOnlyTexture1D
{
    public Texture1D(TextureFormat TextureFormat, int width, int height, int levels = 1)
        : this()
    {
        this.AllocateStorage(TextureFormat, width, height, levels);
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

    public void AllocateStorage(TextureFormat TextureFormat, int width, int height, int levels = 1)
    {
        this.Storage(TextureFormat, width, height, 1, levels);
    }

    #region Update
    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, List<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, Span<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, T[] pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public unsafe void Update<T>(int width, PixelFormat PixelFormat, PixelType PixelType, in T pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.One, width, 1, 1, PixelFormat, PixelType, pixels, level, xOffset);
    }

    public void Update(int width, PixelFormat PixelFormat, PixelType PixelType, IntPtr pixels, int level = 0, int xOffset = 0)
    {
        this.UpdateSubData(TextureDimension.One, width, 0, 0, PixelFormat, PixelType, pixels, level, xOffset);
    }
    #endregion

    #region UpdateCompress
    public void UpdateCompress<T>(int width, PixelFormat PixelFormat, int imageSize, List<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
    }

    public void UpdateCompress<T>(int width, PixelFormat PixelFormat, int imageSize, Span<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
    }

    public void UpdateCompress<T>(int width, PixelFormat PixelFormat, int imageSize, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
    }

    public void UpdateCompress<T>(int width, PixelFormat PixelFormat, int imageSize, T[] pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
    }

    public void UpdateCompress<T>(int width, PixelFormat PixelFormat, int imageSize, in T pixels, int level = 0, int xOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
    }

    public void UpdateCompress(int width, PixelFormat PixelFormat, int imageSize, nuint pixels, int level = 0, int xOffset = 0)
    {
        this.UpdateSubDataCompress(TextureDimension.One, width, 1, 1, PixelFormat, imageSize, pixels, level, xOffset, 0, 0);
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
            $"Filtering: {this.Filtering}\n" +
            $"Wrapping: {this.WrapModeS}\n";
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width,
        int srcLevel, int srcX,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : IReadOnlyTexture
    {
        return this.CopySubData(
            dstTexture,
            width, 1, 1,
            srcLevel, srcX, 0, 0,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture1D CloneGPU()
    {
        Texture1D dstTexture = new Texture1D(this.Format, base.Width, this.Levels);

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
