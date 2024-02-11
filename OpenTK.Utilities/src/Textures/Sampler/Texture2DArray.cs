﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public class Texture2DArray() : TexturesImplements(TextureTarget3d.Texture2DArray), ITexture3D
{
    public Texture2DArray(SizedInternalFormat SizedInternalFormat, int width, int height, int layers, int levels = 1)
        : this()
    {
        this.NewAllocation(SizedInternalFormat, width, height, layers, levels);
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

    public virtual void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int layers, int levels = 1)
    {
        this.NewAllocation(SizedInternalFormat, width, height, layers, levels);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update<T>(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
    }

    public virtual void Update(int width, int height, int layer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
    {
        this.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, zOffset);
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
        Texture2DArray dstTexture = new Texture2DArray(this.InternalFormat, base.Width, base.Height, this.Depth, this.Levels);

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
