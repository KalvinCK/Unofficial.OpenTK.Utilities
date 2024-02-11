using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

public abstract class TexturesImplements : ITexture, IDisposable
{
    private long textureBindlessHandler = 0;

    public TexturesImplements(TextureTarget1d allocateToTarget1DTexture)
    {
        this.Dimension = TextureDimension.One;
        this.Target = (TextureTarget)allocateToTarget1DTexture;
        this.CreateNewTextureBuffer();
    }

    public TexturesImplements(TextureTarget2d allocateToTarget2DTexture)
    {
        this.Dimension = TextureDimension.Two;
        this.Target = (TextureTarget)allocateToTarget2DTexture;
        this.CreateNewTextureBuffer();
    }

    public TexturesImplements(TextureTarget3d allocateToTarget3DTexture)
    {
        this.Dimension = TextureDimension.Three;
        this.Target = (TextureTarget)allocateToTarget3DTexture;
        this.CreateNewTextureBuffer();
    }

    /// <summary>
    /// Gets Handler texture.
    /// <para>
    /// #extension: GL_ARB_bindless_texture : required.
    /// </para>
    /// </summary>
    public long BindlessHandler
    {
        get
        {
            if (this.textureBindlessHandler is 0)
            {
                this.textureBindlessHandler = GL.Arb.GetTextureHandle(this.BufferID);
                GL.Arb.MakeTextureHandleResident(this.textureBindlessHandler);
            }

            return this.textureBindlessHandler;
        }
    }

    public TextureDimension Dimension { get; }

    public TextureTarget Target { get; }

    public SizedInternalFormat InternalFormat { get; private set; } = 0;

    public int BufferID { get; private set; } = 0;

    public bool HasAllocated { get; private set; } = false;

    public int Levels { get; private set; } = 0;

    public TextureFiltering Filtering
    {
        get => this.TextureFiltering;
        set
        {
            if (this.Target == TextureTarget.TextureRectangle)
            {
                var filters = new[] { 9728, 9729 };
                var resultMin = filters.Contains((int)value.MinFilter);
                var resultMag = filters.Contains((int)value.MagFilter);

                if (!resultMin && !resultMag)
                {
                    Helpers.PrintWarning("Rectangle texture do not support mipmaps filtering.");
                    return;
                }
            }

            this.TextureFiltering = value;
            GL.TextureParameter(this.BufferID, TextureParameterName.TextureMinFilter, (int)value.MinFilter);
            GL.TextureParameter(this.BufferID, TextureParameterName.TextureMagFilter, (int)value.MagFilter);

            if (value.GenerateMipmap)
            {
                this.GenerateMipmap();
            }
        }
    }

    internal TextureFiltering TextureFiltering { get; set; } = default;

    internal TextureWrapMode WrapModeS { get; set; } = 0;

    internal TextureWrapMode WrapModeT { get; set; } = 0;

    internal TextureWrapMode WrapModeR { get; set; } = 0;

    internal int Width { get; private set; } = 1;

    internal int Height { get; private set; } = 1;

    internal int Depth { get; private set; } = 1;

    public void ClearContext()
    {
        GL.BindTexture(this.Target, 0);
    }

    public void Bind()
    {
        GL.BindTexture(this.Target, this.BufferID);
    }

    public void BindToUnit(int unit)
    {
        GL.BindTextureUnit(unit, this.BufferID);
    }

    public void BindToImageUnit(int unit, int level, bool layered, int layer, TextureAccess TextureAccess)
    {
        GL.BindImageTexture(unit, this.BufferID, level, layered, layer, TextureAccess, this.InternalFormat);
    }

    #region Sets
    public void SetMipmapLodBias(float bias)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureLodBias, bias);
    }

    public unsafe void SetBorderColor(System.Numerics.Vector4 color)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureBorderColor, &color.X);
    }

    public unsafe void SetBorderColor(Vector4 color)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureBorderColor, &color.X);
    }

    public unsafe void SetBorderColor(Color4 color)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureBorderColor, &color.R);
    }

    public void SetSwizzleR(All swizzle)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureSwizzleR, (int)swizzle);
    }

    public void SetSwizzleG(All swizzle)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureSwizzleG, (int)swizzle);
    }

    public void SetSwizzleB(All swizzle)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureSwizzleB, (int)swizzle);
    }

    public void SetSwizzleA(All swizzle)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureSwizzleA, (int)swizzle);
    }

    public void SetAnisotropy(float value)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureMaxAnisotropy, value);
    }

    public void SetCompareMode(TextureCompareMode textureCompareMode)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureCompareMode, (int)textureCompareMode);
    }

    public void SetCompareFunc(All textureCompareFunc)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureCompareFunc, (int)textureCompareFunc);
    }

    public void SetMaxLevel(int value)
    {
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureMaxLevel, value);
    }

    #endregion

    public void GetImageData(PixelFormat PixelFormat, PixelType PixelType, IntPtr pixels, int bufSize, int level = 0)
    {
        GL.GetTextureImage(this.BufferID, level, PixelFormat, PixelType, bufSize, pixels);
    }

    public unsafe void Clear<TData>(PixelFormat PixelFormat, PixelType PixelType, in TData value, int level = 0)
        where TData : unmanaged
    {
        fixed (void* ptr = &value)
        {
            this.Clear(PixelFormat, PixelType, (IntPtr)ptr, level);
        }
    }

    public void Clear(PixelFormat PixelFormat, PixelType PixelType, IntPtr value, int level = 0)
    {
        GL.ClearTexImage(this.BufferID, level, PixelFormat, PixelType, value);
    }

    public void BecomeNonResident()
    {
        if (this.textureBindlessHandler is not 0)
        {
            GL.Arb.MakeTextureHandleNonResident(this.textureBindlessHandler);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal void SetWrapModeS(TextureWrapMode wrapS)
    {
        this.WrapModeS = wrapS;
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureWrapS, (int)wrapS);
    }

    internal void SetWrapModeT(TextureWrapMode wrapT)
    {
        this.WrapModeT = wrapT;
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureWrapT, (int)wrapT);
    }

    internal void SetWrapModeR(TextureWrapMode wrapR)
    {
        this.WrapModeR = wrapR;
        GL.TextureParameter(this.BufferID, TextureParameterName.TextureWrapR, (int)wrapR);
    }

    internal void GetSizeMipmap(out int width, out int height, out int depth, int level = 0)
    {
        GL.GetTextureLevelParameter(this.BufferID, level, GetTextureParameter.TextureWidth, out width);
        GL.GetTextureLevelParameter(this.BufferID, level, GetTextureParameter.TextureHeight, out height);
        GL.GetTextureLevelParameter(this.BufferID, level, GetTextureParameter.TextureDepth, out depth);
    }

    internal bool CopySubData<TTexture>(
        TTexture dstTexture,
        int width,
        int height,
        int depth,
        int srcLevel, int srcX, int srcY, int srcZ,
        int dstLevel, int dstX, int dstY, int dstZ)
        where TTexture : ITexture
    {
        if (!this.HasAllocated || !dstTexture.HasAllocated)
        {
            return false;
        }

        var srcBuffer = this.BufferID;
        var dstBuffer = dstTexture.BufferID;

        var dstTarget = (ImageTarget)dstTexture.Target;
        var srcTarget = (ImageTarget)this.Target;

        GL.CopyImageSubData(
            srcBuffer, srcTarget, srcLevel, srcX, srcY, srcZ,
            dstBuffer, dstTarget, dstLevel, dstX, dstY, dstZ,
            width, height, depth);

        return true;
    }

    #region Storage
    internal void NewAllocation(SizedInternalFormat SizedInternalFormat, int width = 1, int height = 1, int depth = 1, int levels = 1)
    {
        width = Math.Max(width, 1);
        height = Math.Max(height, 1);
        depth = Math.Max(depth, 1);
        levels = Math.Max(levels, 1);

        if ((this.Width == width && this.Height == height && this.Depth == depth && this.Levels == levels && this.InternalFormat == SizedInternalFormat) && this.HasAllocated)
        {
            return;
        }
        else
        {
            this.CreateNewTextureBuffer();
        }

        switch (this.Dimension)
        {
            case TextureDimension.One:
                GL.TextureStorage1D(this.BufferID, levels, SizedInternalFormat, width);
                break;

            case TextureDimension.Two:
                GL.TextureStorage2D(this.BufferID, levels, SizedInternalFormat, width, height);
                break;

            case TextureDimension.Three:
                GL.TextureStorage3D(this.BufferID, levels, SizedInternalFormat, width, height, depth);
                break;
        }

        this.InternalFormat = SizedInternalFormat;
        this.Levels = levels;
        this.Width = width;
        this.Height = height;
        this.Depth = depth;
        this.HasAllocated = true;
    }

    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        Span<T> span = CollectionsMarshal.AsSpan(pixels);
        if (span.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }

    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        if (pixels.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }

    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, ReadOnlySpan<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        if (pixels.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }

    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        if (pixels.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }

    internal unsafe void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
        where T : unmanaged
    {
        fixed (void* ptr = &pixels)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, PixelFormat, PixelType, (nint)ptr, level, xOffset, yOffset, zOffset);
        }
    }

    internal void UpdatePixels(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat PixelFormat, PixelType PixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
    {
        switch (TextureDimension)
        {
            case TextureDimension.One:
                GL.TextureSubImage1D(this.BufferID, level, xOffset, width, PixelFormat, PixelType, pixels);
                break;
            case TextureDimension.Two:
                GL.TextureSubImage2D(this.BufferID, level, xOffset, yOffset, width, height, PixelFormat, PixelType, pixels);
                break;
            case TextureDimension.Three:
                GL.TextureSubImage3D(this.BufferID, level, xOffset, yOffset, zOffset, width, height, depth, PixelFormat, PixelType, pixels);
                break;
        }
    }
    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.BecomeNonResident();
            GL.DeleteTexture(this.BufferID);

            // Reset Values
            this.BufferID = 0;
            this.HasAllocated = false;
            this.InternalFormat = 0;
            this.Levels = 0;
            this.Width = 1;
            this.Height = 1;
            this.Depth = 1;
            this.TextureFiltering = default;
            this.WrapModeS = 0;
            this.WrapModeT = 0;
            this.WrapModeR = 0;
            this.textureBindlessHandler = 0;
        }
    }

    private void GenerateMipmap()
    {
        if (this.Target != TextureTarget.TextureRectangle)
        {
            Helpers.PrintWarning("Filters related to mipmaps cannot be created applied to a Rectangle texture.");
            return;
        }

        if (this.HasAllocated is false)
        {
            Helpers.PrintWarning("Mips maps cannot be applied to textures that have been allocated.");
            return;
        }
        else if (this.Levels <= 1)
        {
            Helpers.PrintWarning("It doesn't make sense and it's unnecessary to use mip maps with just 1 level allocated.");
            return;
        }
        else
        {
            GL.GenerateTextureMipmap(this.BufferID);
        }
    }

    private void CreateNewTextureBuffer()
    {
        if (this.BufferID != 0)
        {
            this.Dispose(true);
        }

        this.BufferID = ITexture.CreateTextureBuffer(this.Target);
    }
}
