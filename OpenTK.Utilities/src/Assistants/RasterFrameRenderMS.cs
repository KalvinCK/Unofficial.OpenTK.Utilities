using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Objects;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Assistants;

public class RasterFrameRenderMS : IDisposable
{
    private FramebufferObject frameCapture;
    private RenderbufferObject renderCapture;
    private Texture2DMultisampler textureMSCapture;
    private FramebufferObject frameSampler;
    private Texture2D textureSampler;
    private TextureFormat format;
    private Size size;
    private TextureFiltering textureFiltering = TextureFiltering.Nearest;
    private Texture2DWrapping textureWrapping = Texture2DWrapping.ClampToEdge;
    private int numSamples = 4;

    public RasterFrameRenderMS(Size initSize, TextureFormat internalFormat = TextureFormat.Rgba16f)
    {
        this.frameCapture = new FramebufferObject();
        this.renderCapture = new RenderbufferObject();
        this.textureMSCapture = new Texture2DMultisampler();

        this.frameSampler = new FramebufferObject();
        this.textureSampler = new Texture2D();

        this.format = internalFormat;
        this.Size = initSize;
    }

    public TextureFormat InternalFormat
    {
        get => this.format;
        set
        {
            this.format = value;

            this.textureMSCapture.ToAllocate(this.format, this.size.Width, this.size.Height, this.numSamples, true);
            this.frameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureMSCapture);

            this.textureSampler.AllocateStorage(this.format, this.size.Width, this.size.Height);
            this.textureSampler.Filtering = this.textureFiltering;
            this.textureSampler.Wrapping = this.textureWrapping;
            this.frameSampler.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureSampler);
        }
    }

    public Size Size
    {
        get => this.size;
        set
        {
            this.size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            this.textureMSCapture.ToAllocate(this.format, this.size.Width, this.size.Height, this.numSamples, true);
            this.frameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureMSCapture);

            this.renderCapture.StorageMultisampler(RenderbufferStorage.Depth24Stencil8, this.size.Width, this.size.Height, this.numSamples);
            this.frameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, this.renderCapture);

            // Texture2D result
            this.textureSampler.AllocateStorage(this.format, this.size.Width, this.size.Height);
            this.textureSampler.Filtering = this.textureFiltering;
            this.textureSampler.Wrapping = this.textureWrapping;
            this.frameSampler.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureSampler);
        }
    }

    public TextureFiltering Filtering
    {
        get => this.textureFiltering;
        set
        {
            this.textureFiltering = value;
            this.textureSampler.Filtering = this.textureFiltering;
        }
    }

    public Texture2DWrapping Wrapping
    {
        get => this.textureWrapping;
        set
        {
            this.textureWrapping = value;
            this.textureSampler.Wrapping = this.textureWrapping;
        }
    }

    public int SamplesCount
    {
        get => this.numSamples;
        set
        {
            int maxSamples = GL.GetInteger(GetPName.MaxSamples);
            this.numSamples = Math.Clamp(value, 1, maxSamples);

            if (value > maxSamples)
            {
                Helpers.Print($"Max Samplers is: [{maxSamples}]");
            }

            this.textureMSCapture.ToAllocate(this.format, this.size.Width, this.size.Height, this.numSamples, true);
            this.frameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureMSCapture);

            this.renderCapture.StorageMultisampler(RenderbufferStorage.Depth24Stencil8, this.size.Width, this.size.Height, this.numSamples);
            this.frameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, this.renderCapture);
        }
    }

    public IReadOnlyTexture2D FrameResult
    {
        get
        {
            this.frameCapture.Blit(this.frameSampler, this.textureSampler.Width, this.textureSampler.Height, BlitFramebufferFilter.Nearest, ClearBufferMask.ColorBufferBit);
            return this.textureSampler;
        }
    }

    public void BindToDrawing()
    {
        this.frameCapture.Bind();
    }

    public void BindToDrawingAndClear(ClearBufferMask ClearBufferMask)
    {
        this.frameCapture.BindAndClear(ClearBufferMask);
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
            this.textureMSCapture.Dispose();
            this.frameCapture.Dispose();
            this.renderCapture.Dispose();

            this.frameSampler.Dispose();
            this.textureSampler.Dispose();

            this.format = 0;
            this.size = default;
            this.textureFiltering = default;
            this.textureWrapping = default;
            this.numSamples = 0;
        }
    }
}
