using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class Texture1DArray() : TexturesImplements(TextureTarget2d.Texture1DArray), ITexture2D
{
    public int Levels => base._Levels;
    public int Width => base._Width;
    public int Height => base._Height;
    public Size Size => new Size(_Width, _Height);
    public TextureMinFilter MinFilter => base._MinFilter;
    public TextureMagFilter MagFilter => base._MagFilter;
    public TextureWrapMode WrapModeS => base._WrapModeS;
    public TextureWrapMode WrapModeT => base._WrapModeT;

    public Texture1DArray(SizedInternalFormat SizedInternalFormat, int width, int layers, int levels = 1) : this()
    {
        base.NewAllocation(SizedInternalFormat, width, layers, 1, levels);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int layers, int levels = 1)
    {
        base.NewAllocation(SizedInternalFormat, width, layers, 1, levels);
    }
    public Vector2i GetSizeMipmap(int level = 0)
    {
        base.GetSizeMipmap(out var width, out var layer, out _, level);
        return new Vector2i(width, layer);
    }
    public void GetSizeMipmap(out int width, out int layer, int level = 0)
    {
        base.GetSizeMipmap(out width, out layer, out _, level);
    }
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT)
    {
        base.SetWrapMode(wrapS, wrapT);
    }
    public void SetWrapping(Texture2DWrapping wrapping)
    {
        base.SetWrapMode(wrapping.WrapModeS, wrapping.WrapModeT);
    }
    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public unsafe void Update<T>(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update(int width, int layer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        base.UpdatePixels(TextureDimension.Two, width, layer, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }

    public bool CopyGPU<TTexture>(TTexture dstTexture,
        int width, int layers,
        int srcLevel, int srcX, int srcLayer, 
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        return base.CopySubData(dstTexture,
            width, layers, 1,
            srcLevel, srcX, srcLayer, 0,
            dstLevel, dstX, dstY, dstZ);
    }
    public Texture2D CloneGPU(bool clone_Wrap_Samples = true)
    {
        Texture2D dstTexture = new Texture2D(_InternalFormat, _Width, _Height, _Levels);

        if (clone_Wrap_Samples)
        {
            base.CopyFilters(dstTexture);
            base.CopyWrapModes(dstTexture);
        }

        for (int level = 0; level < _Levels; level++)
        {
            base.CopySubData(dstTexture,
            _Width, _Height, 1,
            level, 0, 0, 0,
            level, 0, 0, 0);
        }
        return dstTexture;
    }
}
