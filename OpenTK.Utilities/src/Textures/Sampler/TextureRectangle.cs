using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Textures;

public class TextureRectangle : ITexture2D, IDisposable
{
    private SizedInternalFormat _InternalFormat = 0;
    private bool _HasAllocated = false;
    private int _BufferID = 0;
    private int _Levels = 1;
    private int _Width = 1;
    private int _Height = 1;
    private int _Depth = 1;
    private long _TextureBindlessHandler = 0;
    private long _SamplerBindlessHandler = 0;
    private TextureMinFilter _MinFilter = TextureMinFilter.Nearest;
    private TextureMagFilter _MagFilter = TextureMagFilter.Nearest;
    private TextureWrapMode _WrapModeS = TextureWrapMode.ClampToEdge;
    private TextureWrapMode _WrapModeT = TextureWrapMode.ClampToEdge;


    public TextureDimension Dimension { get; } = TextureDimension.Two;
    public TextureTarget Target { get; } = TextureTarget.TextureRectangle;
    public SizedInternalFormat InternalFormat => _InternalFormat;
    public int BufferID => _BufferID;
    public bool HasAllocated => _HasAllocated;
    public int Levels => _Levels;

    public int Width => _Width;
    public int Height => _Height;
    public Size Size => new Size(_Width, _Height);
    public TextureMinFilter MinFilter => _MinFilter;
    public TextureMagFilter MagFilter => _MagFilter;
    public TextureWrapMode WrapModeS => _WrapModeS;
    public TextureWrapMode WrapModeT => _WrapModeT;

    public TextureRectangle()
    {
        CreateNewTextureBuffer();
    }
    public TextureRectangle(SizedInternalFormat SizedInternalFormat, int width, int height) : this()
    {
        this.ToAllocate(SizedInternalFormat, width, height);
    }

    #region Storage
    public void ToAllocate(SizedInternalFormat SizedInternalFormat, int width, int height)
    {
        if (CompareNewParams(SizedInternalFormat, width, height) && _HasAllocated)
        {
            return;
        }
        else
        {
            CreateNewTextureBuffer();
        }

        _InternalFormat = SizedInternalFormat;
        _Width = width; _Height = height;
        _HasAllocated = true;

        GL.TextureStorage2D(_BufferID, 1, _InternalFormat, _Width, _Height);

    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, List<T> pixels, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        this.Update(width, height, pixelFormat, pixelType, pixels[0], xOffset, yOffset);
    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, Span<T> pixels, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        this.Update(width, height, pixelFormat, pixelType, pixels[0], xOffset, yOffset);
    }
    public void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, T[] pixels, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        this.Update(width, height, pixelFormat, pixelType, pixels[0], xOffset, yOffset);
    }
    public unsafe void Update<T>(int width, int height, PixelFormat pixelFormat, PixelType pixelType, in T pixels, int xOffset = 0, int yOffset = 0) where T : unmanaged
    {
        fixed (void* ptr = &pixels)
        {
            this.Update(width, height, pixelFormat, pixelType, (nint)ptr, xOffset, yOffset);
        }
    }
    public void Update(int width, int height, PixelFormat pixelFormat, PixelType pixelType, nint pixels, int xOffset = 0, int yOffset = 0)
    {
        GL.TextureSubImage2D(_BufferID, 0, xOffset, yOffset, width, height, pixelFormat, pixelType, pixels);
    }
    #endregion

    #region State
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
            _Width = 1;
            _Height = 1;
            _TextureBindlessHandler = 0;
            _SamplerBindlessHandler = 0;
            _MinFilter = TextureMinFilter.Nearest;
            _MagFilter = TextureMagFilter.Nearest;
            _WrapModeS = TextureWrapMode.ClampToEdge;
            _WrapModeT = TextureWrapMode.ClampToEdge;
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

    #region Gets
    public void GetImageData(PixelFormat pixelFormat, PixelType pixelType, IntPtr pixels, int bufSize, int level = 0)
    {
        GL.GetTextureImage(_BufferID, level, pixelFormat, pixelType, bufSize, pixels);
    }
    #endregion

    #region Sets
    public void SetFiltering(TextureFiltering filtering)
    {
        bool result = new[] { TextureMinFilter.Linear, TextureMinFilter.Nearest}.Contains(filtering.MinFilter);

        if (result)
        {
            _MinFilter = filtering.MinFilter; _MagFilter = filtering.MagFilter;
            GL.TextureParameter(_BufferID, TextureParameterName.TextureMinFilter, (int)filtering.MinFilter);
            GL.TextureParameter(_BufferID, TextureParameterName.TextureMagFilter, (int)filtering.MagFilter);
        }
    }
    public void SetFiltering(TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        bool result = new[] { TextureMinFilter.Linear, TextureMinFilter.Nearest }.Contains(minFilter);

        if (result)
        {
            _MinFilter = minFilter; _MagFilter = magFilter;
            GL.TextureParameter(_BufferID, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TextureParameter(_BufferID, TextureParameterName.TextureMagFilter, (int)magFilter);
        }
    }
    public void SetWrapping(TextureWrapMode wrapS, TextureWrapMode wrapT)
    {
        _WrapModeS = wrapS; _WrapModeT = wrapT;
        GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapS, (int)WrapModeS);
        GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapT, (int)WrapModeT);
    }
    public void SetWrapping(Texture2DWrapping wrapping)
    {
        _WrapModeS = wrapping.WrapModeS; _WrapModeT = wrapping.WrapModeT;
        GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapS, (int)wrapping.WrapModeS);
        GL.TextureParameter(_BufferID, TextureParameterName.TextureWrapT, (int)wrapping.WrapModeT);
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
    #endregion

    #region ###
    private bool CompareNewParams(SizedInternalFormat SizedInternalFormat, int width, int height)
    {
        return (_Width == width && _Height == height && InternalFormat == SizedInternalFormat);
    }
    public bool CompareParams<TTexture>(TTexture texture) where TTexture : TextureRectangle, ITexture
    {
        return CompareNewParams(texture.InternalFormat, texture._Width, texture._Height);
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
    public TextureRectangle Recort(SizedInternalFormat internalFormat, int x, int y, int width, int height, bool UseCanvasCoords = false)
    {
        int size_Width = width - x;
        int Size_Height = height - y;

        if(UseCanvasCoords)
        {
            y = (_Height - y) - Size_Height;
        }

        TextureRectangle dstTexture = new TextureRectangle(internalFormat, size_Width, Size_Height);

        int srcBuffer = _BufferID;
        int dstBuffer = dstTexture._BufferID;

        GL.CopyImageSubData(
            srcBuffer, ImageTarget.TextureRectangle, 0, x, y, 0, // src
            dstBuffer, ImageTarget.TextureRectangle, 0, 0, 0, 0, // dst
            width, height, 1);

        return dstTexture;
    }
    public bool CopyGPU<TTexture>(TTexture dstTexture,
        int width, int height,
        int srcX, int srcY,
        int dstLevel, int dstX, int dstY, int dstZ) where TTexture : ITexture
    {
        if (!_HasAllocated || !dstTexture.HasAllocated)
        {
            return false;
        }

        int srcBuffer = _BufferID;
        int dstBuffer = dstTexture.BufferID;

        ImageTarget srcTarget = (ImageTarget)Target;
        ImageTarget dstTarget = (ImageTarget)dstTexture.Target;

        GL.CopyImageSubData(
            srcBuffer, srcTarget, 0, srcX, srcY, 0,
            dstBuffer, dstTarget, dstLevel, dstX, dstY, dstZ,
            width, height, 1);

        return true;
    }
    public TextureRectangle CloneGPU()
    {
        TextureRectangle dstTexture = new TextureRectangle(_InternalFormat, _Width, _Height);

        int srcBuffer = _BufferID;
        int dstBuffer = dstTexture._BufferID;

        GL.CopyImageSubData(
            srcBuffer, ImageTarget.TextureRectangle, 0, 0, 0, 0,
            dstBuffer, ImageTarget.TextureRectangle, 0, 0, 0, 0,
            _Width, _Height, 1);

        return dstTexture;
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
    private void BecomeNonResident()
    {
        if (_TextureBindlessHandler is not 0) GL.Arb.MakeTextureHandleNonResident(_TextureBindlessHandler);
        if (_SamplerBindlessHandler is not 0) GL.Arb.MakeTextureHandleNonResident(_SamplerBindlessHandler);
    }
    #endregion


}
