using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenTK.Utilities.Textures;
public class TextureCubeMap() : TexturesImplements(TextureTarget2d.TextureCubeMap), ITexture3D
{
    public int Levels => base._Levels;
    public int Width => base._Width;
    public int Height => base._Height;
    public int Layers { get; } = 6;
    public Vector3i Size => new Vector3i(_Width, _Height, 6);
    public TextureMinFilter MinFilter => base._MinFilter;
    public TextureMagFilter MagFilter => base._MagFilter;
    public TextureWrapMode WrapModeS => base._WrapModeS;
    public TextureWrapMode WrapModeT => base._WrapModeT;
    public TextureWrapMode WrapModeR => base._WrapModeR;

    public TextureCubeMap(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1) : this()
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height, int levels = 1)
    {
        base.NewAllocation(SizedInternalFormat, width, height, 1, levels);
    }
    /// <summary>
    /// Required extension: GL_AMD_seamless_cubemap_per_texture
    /// </summary>
    /// <param name="state"></param>
    public void EnableSeamlessCubemapARB_AMD(bool state)
    {
        GL.TextureParameter(BufferID, TextureParameterName.TextureCubeMapSeamless, state ? 1 : 0);
    }
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT, TextureWrapMode wrapR)
    {
        base.SetWrapMode(wrapS, wrapT, wrapR);
    }
    public void SetWrapping(Texture3DWrapping wrapping)
    {
        base.SetWrapMode(wrapping.WrapModeS, wrapping.WrapModeT, wrapping.WrapModeR);
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
    public void Update<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public unsafe void Update<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        base.UpdatePixels(TextureDimension.Three, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }
    public void Update(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0)
    {
        base.UpdatePixels(TextureDimension.Three, width, height, 1, pixelFormat, pixelType, pixels, level, xOffset, yOffset, (int)CubeMapLayer);
    }


    public void Update_AMDDriver<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int border = 0) where T : unmanaged
    {
        Span<T> span = CollectionsMarshal.AsSpan(pixels);
        if (span.Length > 0)
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, pixels, level, border);
        }
        else
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, IntPtr.Zero, level, border);
        }
    }
    public void Update_AMDDriver<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int border = 0) where T : unmanaged
    {
        if(pixels.Length > 0)
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, pixels, level, border);
        }
        else
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, IntPtr.Zero, level, border);
        }
    }
    public void Update_AMDDriver<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int border = 0) where T : unmanaged
    {
        if (pixels.Length > 0)
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, pixels, level, border);
        }
        else
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, IntPtr.Zero, level, border);
        }
    }
    public unsafe void Update_AMDDriver<T>(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int border = 0) where T : unmanaged
    {
        fixed (void* ptr = &pixels)
        {
            this.Update_AMDDriver(width, height, CubeMapLayer, pixelFormat, pixelType, (nint)ptr, level, border);
        }
    }
    public void Update_AMDDriver(int width, int height, CubeMapLayer CubeMapLayer, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int border = 0)
    {
        Bind();
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + (int)CubeMapLayer, level, (PixelInternalFormat)InternalFormat, width, height, border, pixelFormat, pixelType, pixels);
        ClearContext();
    }

    public bool CopyGPU<TTexture>(
        TTexture dstTexture,
        int width, int height,
        int srcLevel, int srcX, int srcY, CubeMapLayer CubeMapLayer,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        return base.CopySubData(dstTexture,
            width, height, 1,
            srcLevel, srcX, srcY, (int)CubeMapLayer,
            dstLevel, dstX, dstY, dstZ);
    }
    public TextureCubeMap CloneGPU(bool clone_Wrap_Samples = true)
    {
        TextureCubeMap dstTexture = new TextureCubeMap(_InternalFormat, _Width, _Height, Levels);

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
