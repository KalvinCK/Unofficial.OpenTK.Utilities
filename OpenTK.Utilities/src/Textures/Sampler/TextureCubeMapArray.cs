using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class TextureCubeMapArray() : TexturesImplements(TextureTarget3d.TextureCubeMapArray), ITexture3D
{
    public int Levels => base._Levels;
    public int Width => base._Width;
    public int Height => base._Height;
    public int Layers => base._Depth;
    public Vector3i Size => new Vector3i(_Width, _Height, _Depth);
    public TextureMinFilter MinFilter => base._MinFilter;
    public TextureMagFilter MagFilter => base._MagFilter;
    public TextureWrapMode WrapModeS => base._WrapModeS;
    public TextureWrapMode WrapModeT => base._WrapModeT;
    public TextureWrapMode WrapModeR => base._WrapModeR;
    public TextureCubeMapArray(SizedInternalFormat SizedInternalFormat, int width, int height, int Layer, int levels = 1) : this()
    {
        base.NewAllocation(SizedInternalFormat, width, height, Layer, levels);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int layer, int levels = 1)
    {
        base.NewAllocation(SizedInternalFormat, width, height, layer, levels);
    }
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT, TextureWrapMode wrapR)
    {
        base.SetWrapMode(wrapS, wrapT, wrapR);
    }
    public void SetWrapping(Texture3DWrapping wrapping)
    {
        base.SetWrapMode(wrapping.WrapModeS, wrapping.WrapModeT, wrapping.WrapModeR);
    }
    public Vector3i GetSizeMipmap(int level = 0)
    {
        base.GetSizeMipmap(out var width, out var height, out var layers, level);
        return new Vector3i(width, height, layers);
    }
    public new void GetSizeMipmap(out int width, out int height, out int layers, int level = 0)
    {
        base.GetSizeMipmap(out width, out height, out layers, level);
    }
    public void Update<T>(int width, int height, int layer, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update<T>(int width, int height, int layer, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update<T>(int width, int height, int layer, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update<T>(int width, int height, int layer, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update(int width, int height, int layer, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        base.UpdatePixels(TextureDimension.Three, width, height, layer, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width, int height, CubeMapLayer CubeMapLayer,
        int srcLevel, int srcX, int srcY, int srcLayer,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        return base.CopySubData(
            dstTexture,
            width, height, (int)CubeMapLayer,
            srcLevel, srcX, srcY, srcLayer,
            dstLevel, dstX, dstY, dstZ);
    }
    public TextureCubeMapArray CloneGPU(bool clone_Wrap_Samples = true)
    {
        TextureCubeMapArray dstTexture = new TextureCubeMapArray(_InternalFormat, _Width, _Height, _Depth, _Levels);

        if (clone_Wrap_Samples)
        {
            base.CopyFilters(dstTexture);
            base.CopyWrapModes(dstTexture);
        }


        for (int level = 0; level < _Levels; level++)
        {
            base.CopySubData(dstTexture,
                _Width, _Height, _Depth,
                level, 0, 0, 0,
                level, 0, 0, 0);
        }

        return dstTexture;
    }
}
