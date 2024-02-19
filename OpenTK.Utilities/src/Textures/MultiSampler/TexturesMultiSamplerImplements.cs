using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public abstract class TexturesMultiSamplerImplements : ITexture, IDisposable
{
    private long textureBindlessHandler = 0;

    public TexturesMultiSamplerImplements(TextureTargetMultisample2d TextureTargetMudltisample2d)
    {
        this.Dimension = TextureDimension.Two;
        this.Target = (TextureTarget)TextureTargetMudltisample2d;
        this.CreateNewTextureBuffer();
    }

    public TexturesMultiSamplerImplements(TextureTargetMultisample3d TextureTargetMultisample3d)
    {
        this.Dimension = TextureDimension.Three;
        this.Target = (TextureTarget)TextureTargetMultisample3d;
        this.CreateNewTextureBuffer();
    }

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

    public TextureFormat Format { get; set; } = 0;

    public bool HasAllocated { get; private set; } = false;

    public int BufferID { get; private set; } = 0;

    public int Samples { get; private set; } = 0;

    public bool FixedSampleLocations { get; private set; } = false;

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
        GL.BindImageTexture(unit, this.BufferID, level, layered, layer, TextureAccess, (SizedInternalFormat)this.Format);
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

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    internal void AllocateTextures(TextureFormat TextureFormat, int width, int height, int depth, int samples, bool fixedSampleLocations)
    {
        if (this.CompareParams(TextureFormat, width, height, depth, samples, fixedSampleLocations) && this.HasAllocated)
        {
            return;
        }
        else
        {
            this.CreateNewTextureBuffer();
        }

        this.Format = TextureFormat;
        this.Width = Math.Max(width, 1);
        this.Height = Math.Max(height, 1);
        this.Depth = Math.Max(depth, 1);
        this.Samples = Math.Max(samples, 1);
        this.FixedSampleLocations = fixedSampleLocations;
        this.HasAllocated = true;

        switch (this.Dimension)
        {
            case TextureDimension.Two:
                GL.TextureStorage2DMultisample(this.BufferID, this.Samples, (SizedInternalFormat)this.Format, this.Width, this.Height, this.FixedSampleLocations);
                break;

            case TextureDimension.Three:
                GL.TextureStorage3DMultisample(this.BufferID, this.Samples, (SizedInternalFormat)this.Format, this.Width, this.Height, this.Depth, this.FixedSampleLocations);
                break;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.BecomeNonResident();
            GL.DeleteTexture(this.BufferID);

            // Reset Values
            this.HasAllocated = false;
            this.Format = 0;
            this.BufferID = 0;
            this.Width = 1;
            this.Height = 1;
            this.Depth = 1;
            this.Samples = 0;
            this.FixedSampleLocations = false;
            this.textureBindlessHandler = 0;
        }
    }

    private void BecomeNonResident()
    {
        if (this.textureBindlessHandler is not 0)
        {
            GL.Arb.MakeTextureHandleNonResident(this.textureBindlessHandler);
        }
    }

    private void CreateNewTextureBuffer()
    {
        this.DeleteTexture();
        this.BufferID = ITexture.CreateTextureBuffer(this.Target);
    }

    private bool CompareParams(TextureFormat TextureFormat, int width, int height, int depth, int samples, bool fixedSampleLocations)
    {
        return this.Width == width && this.Height == height && this.Depth == depth && this.Samples == samples && this.Format == TextureFormat && this.FixedSampleLocations == fixedSampleLocations;
    }

    private void DeleteTexture()
    {
        if (this.BufferID != 0)
        {
            GL.DeleteTexture(this.BufferID);
        }
    }
}
