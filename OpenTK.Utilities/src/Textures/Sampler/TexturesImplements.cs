using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace OpenTK.Utilities.Textures;

// implementation class to help with texture classes
public abstract class TexturesImplements : ITexture, IDisposable
{
    internal SizedInternalFormat _InternalFormat = 0;
    internal bool _HasAllocated = false;
    internal int _BufferID = 0;
    internal int _Levels = 0; 
    internal int _Width  = 1;
    internal int _Height = 1;
    internal int _Depth  = 1;
    internal TextureMinFilter _MinFilter = 0;
    internal TextureMagFilter _MagFilter = 0;
    internal TextureWrapMode _WrapModeS  = 0;
    internal TextureWrapMode _WrapModeT  = 0;
    internal TextureWrapMode _WrapModeR  = 0;
    private long _TextureBindlessHandler = 0;
    private long _SamplerBindlessHandler = 0;


    public TextureDimension Dimension { get; }
    public TextureTarget Target { get; }
    public SizedInternalFormat InternalFormat => _InternalFormat;
    public int BufferID => _BufferID;
    public bool HasAllocated => _HasAllocated;

    public TexturesImplements(TextureTarget1d allocateToTarget1DTexture)
    {
        Dimension = TextureDimension.One;
        Target = (TextureTarget)allocateToTarget1DTexture;
        CreateNewTextureBuffer();
    }
    public TexturesImplements(TextureTarget2d allocateToTarget2DTexture)
    {
        Dimension = TextureDimension.Two;
        Target = (TextureTarget)allocateToTarget2DTexture;
        CreateNewTextureBuffer();
    }
    public TexturesImplements(TextureTarget3d allocateToTarget3DTexture)
    {
        Dimension = TextureDimension.Three;
        Target = (TextureTarget)allocateToTarget3DTexture;
        CreateNewTextureBuffer();
    }

    #region Context
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if(disposing)
        {
            BecomeNonResident();
            GL.DeleteTexture(_BufferID);

            // Reset Values
            _BufferID = 0;
            _HasAllocated = false;
            _InternalFormat = 0;
            _Levels = 0;
            _Width = 1;
            _Height = 1;
            _Depth = 1;
            _MinFilter = 0;
            _MagFilter = 0;
            _WrapModeS = 0;
            _WrapModeT = 0;
            _WrapModeR = 0;
            _TextureBindlessHandler = 0;
            _SamplerBindlessHandler = 0;
        }
    }
    
    private void CreateNewTextureBuffer()
    {
        if (_BufferID != 0)
        {
            Dispose(true);
        }

        _BufferID = ITexture.CreateTextureBuffer(Target);
    }
    public void ClearContext()
    {
        GL.BindTexture(Target, 0);
    }
    public void Bind()
    {
        GL.BindTexture(Target, _BufferID);
    }
    public void BindToUnit(int unit)
    {
        GL.BindTextureUnit(unit, _BufferID);
    }
    public void BindToImageUnit(int unit, int level, bool layered, int layer, TextureAccess TextureAccess)
    {
        GL.BindImageTexture(unit, _BufferID, level, layered, layer, TextureAccess, InternalFormat);
    }
    #endregion

    #region Sets
    public void SetMipmapLodBias(float bias)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureLodBias, bias);
    }
    public unsafe void SetBorderColor(System.Numerics.Vector4 color)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureBorderColor, &color.X);
    }
    public unsafe void SetBorderColor(Vector4 color)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureBorderColor, &color.X);
    }
    public unsafe void SetBorderColor(Color4 color)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureBorderColor, &color.R);
    }
    public void SetSwizzleR(All swizzle)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureSwizzleR, (int)swizzle);
    }
    public void SetSwizzleG(All swizzle)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureSwizzleG, (int)swizzle);
    }
    public void SetSwizzleB(All swizzle)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureSwizzleB, (int)swizzle);
    }
    public void SetSwizzleA(All swizzle)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureSwizzleA, (int)swizzle);
    }
    public void SetAnisotropy(float value)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMaxAnisotropy, value);
    }
    public void SetCompareMode(TextureCompareMode textureCompareMode)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureCompareMode, (int)textureCompareMode);
    }
    public void SetCompareFunc(All textureCompareFunc)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureCompareFunc, (int)textureCompareFunc);
    }
    public void SetMaxLevel(int value)
    {
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMaxLevel, value);
    }
    public void SetFiltering(TextureFiltering filtering)
    {
        _MinFilter = filtering.MinFilter; _MagFilter = filtering.MagFilter;
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMinFilter, (int)filtering.MinFilter);
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMagFilter, (int)filtering.MagFilter);

        if(filtering.GenerateMimap)
        {
            GenerateMipmap();
        }
    }
    public void SetFiltering(TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        _MinFilter = minFilter; _MagFilter = magFilter;
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMinFilter, (int)minFilter);
        GL.TextureParameter(_BufferID, TextureParameterName.TextureMagFilter, (int)magFilter);
    }
    internal void SetWrapMode(
        TextureWrapMode wrapS,
        TextureWrapMode? wrapT = null,
        TextureWrapMode? wrapR = null)
    {
        _WrapModeS = wrapS;
        GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapS, (int)wrapS);

        if (wrapT != null)
        {
            if ((int)wrapT != 0)
            {
                _WrapModeT = (TextureWrapMode)wrapT!;
                GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapT, (int)_WrapModeT);
            }
        }

        if (wrapR != null)
        {
            if ((int)wrapR != 0)
            {
                _WrapModeR = (TextureWrapMode)wrapR!;
                GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapR, (int)_WrapModeR);

            }
        }
    }
    #endregion

    #region Gets
    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0)
    {
        GL.GetTextureImage(_BufferID, level, pixelFormat, pixelType, bufSize, pixels);
    }

    internal void GetSizeMipmap(out int width, out int height, out int depth, int level = 0)
    {
        GL.GetTextureLevelParameter(_BufferID, level, GetTextureParameter.TextureWidth, out width);
        GL.GetTextureLevelParameter(_BufferID, level, GetTextureParameter.TextureHeight, out height);
        GL.GetTextureLevelParameter(_BufferID, level, GetTextureParameter.TextureDepth, out depth);
    }
    private bool CompareNewParams(SizedInternalFormat SizedInternalFormat, int levels, int width, int height, int depth)
    {
        return (_Width == width && _Height == height && _Depth == depth && _Levels == levels && InternalFormat == SizedInternalFormat);
    }
    public bool CompareParams<TTexture>(TTexture texture) where TTexture : TexturesImplements, ITexture
    {
        return CompareNewParams(texture.InternalFormat, texture._Levels, texture._Width, texture._Height, texture._Depth);
    }

    #endregion

    #region Storage
    internal void NewAllocation(SizedInternalFormat SizedInternalFormat, int width = 1, int height = 1, int depth = 1, int levels = 1)
    {
        if (CompareNewParams(SizedInternalFormat, levels, width, height, depth) && _HasAllocated)
        {
            return;
        }
        else
        {
            CreateNewTextureBuffer();
        }

        _InternalFormat = SizedInternalFormat;
        _Levels = levels;
        _Width = width; _Height = height; _Depth = depth;

        switch (Dimension)
        {
            case TextureDimension.One:
                GL.TextureStorage1D(_BufferID, _Levels, _InternalFormat, _Width);
                break;

            case TextureDimension.Two:
                GL.TextureStorage2D(_BufferID, _Levels, _InternalFormat, _Width, _Height);
                break;

            case TextureDimension.Three:
                GL.TextureStorage3D(_BufferID, _Levels, _InternalFormat, _Width, _Height, _Depth);
                break;
        }

        _HasAllocated = true;
    }
    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0) where T : unmanaged
    {
        Span<T> span = CollectionsMarshal.AsSpan(pixels);
        if (span.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }
    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0) where T : unmanaged
    {
        if (pixels.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }
    internal void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0) where T : unmanaged
    {
        if(pixels.Length > 0)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, pixels[0], level, xOffset, yOffset, zOffset);
        }
        else
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, IntPtr.Zero, level, xOffset, yOffset, zOffset);
        }
    }
    internal unsafe void UpdatePixels<T>(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0) where T : unmanaged
    {
        fixed (void* ptr = &pixels)
        {
            this.UpdatePixels(TextureDimension, width, height, depth, pixelFormat, pixelType, (nint)ptr, level, xOffset, yOffset, zOffset);
        }
    }
    internal void UpdatePixels(TextureDimension TextureDimension, int width, int height, int depth, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int level = 0, int xOffset = 0, int yOffset = 0, int zOffset = 0)
    {
        switch(TextureDimension)
        {
            case TextureDimension.One:
                GL.TextureSubImage1D(_BufferID, level, xOffset, width, pixelFormat, pixelType, pixels);
                break;
            case TextureDimension.Two:
                GL.TextureSubImage2D(_BufferID, level, xOffset, yOffset, width, height, pixelFormat, pixelType, pixels);
                break;
            case TextureDimension.Three:
                GL.TextureSubImage3D(_BufferID, level, xOffset, yOffset, zOffset, width, height, depth, pixelFormat, pixelType, pixels);
                break;

        }

    }
    #endregion

    #region ###
    public void GenerateMipmap()
    {
        if(_Levels > 1)
        {
            GL.GenerateTextureMipmap(_BufferID);
        }
        else
        {
            Debug.Print("It doesn't make sense and it's unnecessary to use mip maps with just 1 level allocated.");
        }
    }
    public unsafe void Clear<TData>(PixelFormat PixelFormat, PixelType PixelType, in TData value, int level = 0) where TData : unmanaged
    {
        fixed (void* ptr = &value)
        {
            Clear(PixelFormat, PixelType, (IntPtr)ptr, level);
        }
    }
    public void Clear(PixelFormat PixelFormat, PixelType PixelType, IntPtr value, int level = 0)
    {
        GL.ClearTexImage(_BufferID, level, PixelFormat, PixelType, value);
    }
    public bool CopyFilters<TTexture>(TTexture texture) where TTexture : TexturesImplements, ITexture
    {
        if((int)_MinFilter is not 0)
        {
            texture.SetFiltering(_MinFilter, _MagFilter);
            return true;
        }

        return false;
    }
    public bool CopyWrapModes<TTexture>(TTexture texture) where TTexture : TexturesImplements, ITexture
    {
        if ((int)_WrapModeS is not 0)
        {
            texture.SetWrapMode(_WrapModeS, _WrapModeT, _WrapModeR);
            return true;
        }

        return false;
    }
    internal bool CopySubData<TTexture>(TTexture dstTexture,
        int width, int height, int depth,
        int srcLevel, int srcX, int srcY, int srcZ,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        if(!_HasAllocated || !dstTexture.HasAllocated)
        {
            return false;
        }

        var srcBuffer = _BufferID;
        var dstBuffer = dstTexture.BufferID;

        var dstTarget = (ImageTarget)dstTexture.Target;
        var srcTarget = (ImageTarget)Target;

        GL.CopyImageSubData(
            srcBuffer, srcTarget, srcLevel, srcX, srcY, srcZ,
            dstBuffer, dstTarget, dstLevel, dstX, dstY, dstZ, 
            width, height, depth);

        return true;
    }
    #endregion

    #region Extensions
    /// <summary>
    /// #extension: GL_ARB_bindless_texture : required
    /// </summary>
    public long BindlessHandler
    {
        get
        {
            if (_TextureBindlessHandler is 0)
            {
                _TextureBindlessHandler = GL.Arb.GetTextureHandle(_BufferID);
                GL.Arb.MakeTextureHandleResident(_TextureBindlessHandler);
            }
            return _TextureBindlessHandler;
        }
    }

    /// <summary>
    /// #extension: GL_ARB_bindless_texture : required
    /// </summary>
    public long GetSampler(in SamplerObject samplerObject)
    {
        if (_SamplerBindlessHandler is 0)
        {
            _SamplerBindlessHandler = GL.Arb.GetTextureSamplerHandle(_BufferID, samplerObject.BufferID);
            GL.Arb.MakeTextureHandleResident(_SamplerBindlessHandler);
        }
        return _SamplerBindlessHandler;
    }

    public void BecomeNonResident()
    {
        if (_TextureBindlessHandler is not 0) GL.Arb.MakeTextureHandleNonResident(_TextureBindlessHandler);
        if (_SamplerBindlessHandler is not 0) GL.Arb.MakeTextureHandleNonResident(_SamplerBindlessHandler);
    }
    #endregion
}