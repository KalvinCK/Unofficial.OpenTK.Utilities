using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Objects;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Rasterization;

public class RasterFrameDepthCube : IDisposable
{
    private readonly DepthInternalFormat internalFormat;
    private readonly FrameBufferObject frameBufferDepth;
    private readonly TextureCubeMap textureDepth;
    private readonly SamplerObject samplerObjectDepth;
    private Size size;
    private TextureFiltering textureFiltering = TextureFiltering.Nearest;
    private Texture3DWrapping textureWrapping = Texture3DWrapping.ClampToEdge;
    private TextureFiltering samplerFiltering = TextureFiltering.Linear;
    private Texture3DWrapping samplerWrapping = Texture3DWrapping.ClampToEdge;

    public RasterFrameDepthCube(Size initSize, DepthInternalFormat DepthInternalFormat)
    {
        this.frameBufferDepth = new FrameBufferObject();
        this.textureDepth = new TextureCubeMap();

        this.samplerObjectDepth = new SamplerObject();
        this.samplerObjectDepth.SetWrapping3D(this.samplerWrapping);
        this.samplerObjectDepth.SetFiltering(this.samplerFiltering);
        this.samplerObjectDepth.SetSamplerIntParameter(SamplerParameterName.TextureCompareMode, TextureCompareMode.CompareRefToTexture);
        this.samplerObjectDepth.SetSamplerIntParameter(SamplerParameterName.TextureCompareFunc, All.Less);

        this.internalFormat = DepthInternalFormat;
        this.Size = initSize;
    }

    public ITexture3D TextureCubemapDepth => this.textureDepth;

    public ISamplerObject SamplerDepth => this.samplerObjectDepth;

    public long TextureBindlessHandler => this.textureDepth.BindlessHandler;

    public long SamplerBindlessHandler => this.samplerObjectDepth.BindlessHandler;

    public Size Size
    {
        get => this.size;
        set
        {
            this.size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            this.textureDepth.AllocateStorage((TextureFormat)this.internalFormat, this.size.Width, this.size.Height);
            this.textureDepth.Filtering = this.textureFiltering;
            this.textureDepth.Wrapping = this.textureWrapping;
            this.samplerObjectDepth.AttachTexture(this.textureDepth);

            this.frameBufferDepth.SetTexture(FramebufferAttachment.DepthAttachment, this.textureDepth);
            this.frameBufferDepth.ClearBuffer(ClearBuffer.Depth, 0, 1.0f);
        }
    }

    public TextureFiltering TextureFiltering
    {
        get => this.textureFiltering;
        set
        {
            this.textureFiltering = value;
            this.textureDepth.Filtering = this.textureFiltering;
        }
    }

    public Texture3DWrapping TextureWrapping
    {
        get => this.textureWrapping;
        set
        {
            this.textureWrapping = value;
            this.textureDepth.Wrapping = this.textureWrapping;
        }
    }

    public TextureFiltering SamplerFiltering
    {
        get => this.samplerFiltering;
        set
        {
            this.samplerFiltering = value;
            this.samplerObjectDepth.SetFiltering(this.samplerFiltering);
        }
    }

    public Texture3DWrapping SampleWrapping
    {
        get => this.samplerWrapping;
        set
        {
            this.samplerWrapping = value;
            this.samplerObjectDepth.SetWrapping3D(this.samplerWrapping);
        }
    }

    public void BindToDrawing()
    {
        this.frameBufferDepth.Bind();
    }

    public void BindToDrawingAndClear(ClearBufferMask ClearBufferMask)
    {
        this.frameBufferDepth.BindAndClear(ClearBufferMask);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.frameBufferDepth.Dispose();
            this.textureDepth.Dispose();
            this.samplerObjectDepth.Dispose();
        }
    }
}
