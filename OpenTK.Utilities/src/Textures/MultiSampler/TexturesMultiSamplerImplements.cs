using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public abstract class TexturesMultiSamplerImplements : ITexture, IDisposable
{
    internal SizedInternalFormat _InternalFormat = 0;
    internal bool _HasAllocated = false;
    internal int  _BufferID = 0;
    internal int  _Width  = 1;
    internal int  _Height = 1;
    internal int  _Depth  = 1;
    internal int  _Samples = 0;
    internal bool _FixedSampleLocations = false;
    private long _TextureBindlessHandler = 0;


    public TextureDimension Dimension { get; }
    public TextureTarget Target { get; }
    public SizedInternalFormat InternalFormat => _InternalFormat;
    public int BufferID => _BufferID;
    public bool HasAllocated => _HasAllocated;


    public TexturesMultiSamplerImplements(TextureTargetMultisample2d TextureTargetMudltisample2d)
    {
        Dimension = TextureDimension.Two;
        Target = (TextureTarget)TextureTargetMudltisample2d;
        CreateNewTextureBuffer();
    }
    public TexturesMultiSamplerImplements(TextureTargetMultisample3d TextureTargetMultisample3d)
    {
        Dimension = TextureDimension.Three;
        Target = (TextureTarget)TextureTargetMultisample3d;
        CreateNewTextureBuffer();
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            BecomeNonResident();
            GL.DeleteTexture(_BufferID);

            // Reset Values
            _HasAllocated = false;
            _InternalFormat = 0;
            _BufferID = 0;
            _Width = 1;
            _Height = 1;
            _Depth = 1;
            _Samples = 0;
            _FixedSampleLocations = false;
            _TextureBindlessHandler = 0;
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

    private bool CompareParams(SizedInternalFormat SizedInternalFormat, int width, int height, int depth, int samples, bool fixedSampleLocations)
    {
        return _Width == width && _Height == height && _Depth == depth && _Samples == samples && InternalFormat == SizedInternalFormat && _FixedSampleLocations == fixedSampleLocations;
    }
    internal void AllocateTextures(SizedInternalFormat SizedInternalFormat, int width, int height, int depth, int samples, bool fixedSampleLocations)
    {
        if (CompareParams(SizedInternalFormat, width, height, depth, samples, fixedSampleLocations) && _HasAllocated)
        {
            return;
        }
        else
        {
            CreateNewTextureBuffer();
        }

        _InternalFormat = SizedInternalFormat;
        _Width = width;
        _Height = height;
        _Depth = depth;
        _Samples = samples;
        _FixedSampleLocations = fixedSampleLocations;

        switch (Dimension)
        {
            case TextureDimension.Two:
                GL.TextureStorage2DMultisample(_BufferID, samples, SizedInternalFormat, width, height, fixedSampleLocations);
                break;

            case TextureDimension.Three:
                GL.TextureStorage3DMultisample(_BufferID, samples, SizedInternalFormat, width, height, depth, fixedSampleLocations);
                break;

        }
        
        _HasAllocated = true;
    }
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
    private void BecomeNonResident()
    {
        if (_TextureBindlessHandler is not 0)
        {
            GL.Arb.MakeTextureHandleNonResident(_TextureBindlessHandler);
        }
    }
}
