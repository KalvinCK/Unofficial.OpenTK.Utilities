using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Utilities.Objects;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Assistants;

public class Raster2DDepth : IDisposable
{
    private readonly DepthInternalFormat internalFormat;
    private readonly FramebufferObject frameBufferDepth;
    private readonly Texture2D texture;
    private readonly SamplerObject samplerObject;
    private TextureFiltering samplerFiltering = TextureFiltering.Linear;
    private Texture2DWrapping samplerWrapping = Texture2DWrapping.ClampToEdge;
    private TextureFiltering textureFiltering = TextureFiltering.Nearest;
    private Texture2DWrapping textureWrapping = Texture2DWrapping.ClampToEdge;
    private Size size;

    public Raster2DDepth(Size initSize, DepthInternalFormat DepthInternalFormat)
    {
        this.frameBufferDepth = new FramebufferObject();
        this.texture = new Texture2D();

        this.samplerObject = new SamplerObject();
        this.samplerObject.SetWrapping2D(this.samplerWrapping);
        this.samplerObject.SetFiltering(this.samplerFiltering);
        this.samplerObject.SetSamplerIntParameter(SamplerParameterName.TextureCompareMode, TextureCompareMode.CompareRToTexture);
        this.samplerObject.SetSamplerIntParameter(SamplerParameterName.TextureCompareFunc, All.Less);

        this.internalFormat = DepthInternalFormat;
        this.Size = initSize;
    }

    public IReadOnlyTexture2D TextureCubemapDepth => this.texture;

    public IReadOnlySamplerObject SamplerDepth => this.samplerObject;

    public long TextureBindlessHandler => this.texture.BindlessHandler;

    public long SamplerBindlessHandler => this.samplerObject.BindlessHandler;

    public Size Size
    {
        get => this.size;
        set
        {
            this.size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            this.texture.AllocateStorage((TextureFormat)this.internalFormat, this.size.Width, this.size.Height);
            this.texture.Filtering = this.TextureFiltering;
            this.texture.Wrapping = this.TextureWrapping;
            this.texture.SetBorderColor(Vector4.One);
            this.samplerObject.AttachTexture(this.texture);

            this.frameBufferDepth.SetTexture(FramebufferAttachment.DepthAttachment, this.texture);
            this.frameBufferDepth.ClearBuffer(ClearBuffer.Depth, 0, 1.0f);
        }
    }

    public TextureFiltering TextureFiltering
    {
        get => this.textureFiltering;
        set
        {
            this.textureFiltering = value;
            this.texture.Filtering = this.textureFiltering;
        }
    }

    public Texture2DWrapping TextureWrapping
    {
        get => this.textureWrapping;
        set
        {
            this.textureWrapping = value;
            this.texture.Wrapping = this.textureWrapping;
        }
    }

    public TextureFiltering SamplerFiltering
    {
        get => this.samplerFiltering;
        set
        {
            this.samplerFiltering = value;
            this.samplerObject.SetFiltering(this.samplerFiltering);
        }
    }

    public Texture2DWrapping SampleWrapping
    {
        get => this.samplerWrapping;
        set
        {
            this.samplerWrapping = value;
            this.samplerObject.SetWrapping2D(this.samplerWrapping);
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
            this.texture.Dispose();
            this.samplerObject.Dispose();
        }
    }
}
