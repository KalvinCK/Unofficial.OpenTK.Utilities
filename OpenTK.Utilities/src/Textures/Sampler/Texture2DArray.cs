using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class Texture2DArray() : TexturesImplements(TextureTarget3d.Texture2DArray), ITexture3D
{
    public Texture2DArray(TextureFormat TextureFormat, int width, int height, int layers, int levels = 1)
        : this()
    {
        this.AllocateStorage(TextureFormat, width, height, layers, levels);
    }

    public new int Width => base.Width;

    public new int Height => base.Height;

    public int Layers => this.Depth;

    public Vector3i Size => new Vector3i(base.Width, base.Height, this.Depth);

    public Texture3DWrapping Wrapping
    {
        get => new Texture3DWrapping(this.WrapModeS, this.WrapModeT, this.WrapModeR);
        set
        {
            this.SetWrapModeS(value.WrapModeS);
            this.SetWrapModeT(value.WrapModeT);
            this.SetWrapModeR(value.WrapModeR);
        }
    }

    public Vector3i GetSizeMipmap(int level = 0)
    {
        base.GetSizeMipmap(out var width, out var height, out var depth, level);
        return new Vector3i(width, height, depth);
    }

    public new void GetSizeMipmap(out int width, out int height, out int depth, int level = 0)
    {
        base.GetSizeMipmap(out width, out height, out depth, level);
    }

    public virtual void AllocateStorage(TextureFormat TextureFormat, int width, int height, int layers, int levels = 1)
    {
        this.Storage(TextureFormat, width, height, layers, levels);
    }

    #region Update
    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
    {
        this.UpdateSubData(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }
    #endregion

    #region UpdateCompress
    public void UpdateCompress<T>(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
    }

    public void UpdateCompress<T>(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
    }

    public void UpdateCompress<T>(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
    }

    public void UpdateCompress<T>(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
    }

    public void UpdateCompress<T>(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
    }

    public void UpdateCompress(int width, int height, int layer, PixelFormat PixelFormat, int imageSize, nuint pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
    {
        this.UpdateSubDataCompress(TextureDimension.Three, width, height, layer, PixelFormat, imageSize, pixels, level, xOffset, yOffset, zOffset);
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
            $"Layers: {this.Layers}\n" +
            $"Size: {this.Size}\n" +
            $"Filtering: {this.Filtering}\n" +
            $"Wrapping: {this.Wrapping}\n";
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width, int height, int layer,
        int srcLevel, int srcX, int srcY, int layerOffSet,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : ITexture
    {
        return this.CopySubData(
            dstTexture,
            width, height, layer,
            srcLevel, srcX, srcY, layerOffSet,
            dstLevel, dstX, dstY, dstZ);
    }

    public Texture2DArray CloneGPU()
    {
        Texture2DArray dstTexture = new Texture2DArray(this.Format, base.Width, base.Height, this.Depth, this.Levels);

        for (int level = 0; level < this.Levels; level++)
        {
            this.CopySubData(
                dstTexture,
                base.Width, base.Height, this.Depth,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}
