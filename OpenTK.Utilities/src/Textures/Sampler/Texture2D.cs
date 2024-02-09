using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class Texture2D() : TexturesImplements(TextureTarget2d.Texture2D), ITexture2D
{
    public int Levels => base._Levels;
    public int Width => base._Width;
    public int Height => base._Height;
    public Size Size => new Size(Width, Height);
    public TextureMinFilter MinFilter => base._MinFilter;
    public TextureMagFilter MagFilter => base._MagFilter;
    public TextureWrapMode WrapModeS => base._WrapModeS;
    public TextureWrapMode WrapModeT => base._WrapModeT;
    public Texture2D(PixelData<float> pixelData, int levels, PixelDescription description, TextureFiltering filtering, Texture2DWrapping wraping) : this()
    {
        ToAllocate(description.InternalFormat, pixelData.Width, pixelData.Height, levels);
        Update(pixelData.Width, pixelData.Height, description.Format, description.Type, pixelData.Data);
    }
    public Texture2D(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1) : this()
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1)
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    public Vector2i GetSizeMipmap(int level = 0)
    {
        base.GetSizeMipmap(out var width, out var height, out _, level);
        return new Vector2i(width, height);
    }
    public void GetSizeMipmap(out int width, out int height, int level = 0)
    {
        base.GetSizeMipmap(out width, out height, out _, level);
    }
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT)
    {
        base.SetWrapMode(wrapS, wrapT);
    }
    public void SetWrapping(Texture2DWrapping wrapping)
    {
        base.SetWrapMode(wrapping.WrapModeS, wrapping.WrapModeT);
    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public unsafe void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public void Update(int width, int height, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        base.UpdatePixels(TextureDimension.Two, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset);
    }
    public bool CopyGPU<TTexture>(TTexture dstTexture,
        int width, int height,
        int srcLevel, int srcX, int srcY,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        return base.CopySubData(
            dstTexture, 
            width, height, 1, 
            srcLevel, srcX, srcY, 0, 
            dstLevel, dstX, dstY, dstZ);
    }
    public Texture2D CloneGPU()
    {
        Texture2D dstTexture = new Texture2D(InternalFormat, Width, Height, Levels);
        if(!HasAllocated)
        {
            throw new Exception($"Texture is empty");
        }

        base.CopyFilters(dstTexture);
        base.CopyWrapModes(dstTexture);

        for (int level = 0; level < Levels; level++)
        {
            base.CopySubData(dstTexture,
                Width, Height, 1,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}
