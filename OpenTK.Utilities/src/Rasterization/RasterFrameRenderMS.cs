using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;
using System.Drawing;

namespace OpenTK.Utilities.Rasterization;

public class RasterFrameRenderMS : IDisposable
{
    private readonly FrameBufferObject FrameCapture;
    private readonly RenderBufferObject RenderCapture;
    private readonly Texture2DMultiSampler TextureMSCapture;

    private readonly FrameBufferObject FrameSampler;
    private readonly Texture2D TextureSampler;
    
    public RasterFrameRenderMS(Size initSize, SizedInternalFormat internalFormat = SizedInternalFormat.Rgba16f)
    {
        FrameCapture = new FrameBufferObject();
        RenderCapture = new RenderBufferObject();
        TextureMSCapture = new Texture2DMultiSampler();

        FrameSampler = new FrameBufferObject();
        TextureSampler = new Texture2D();

        _InternalFormat = internalFormat;
        Size = initSize;
    }
    private SizedInternalFormat _InternalFormat;
    public SizedInternalFormat InternalFormat
    {
        get => _InternalFormat;
        set
        {
            _InternalFormat = value;

            TextureMSCapture.ToAllocate(_InternalFormat, _Size.Width, _Size.Height, _NumSamples, true);
            FrameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, TextureMSCapture);

            TextureSampler.ToAllocate(_InternalFormat, _Size.Width, _Size.Height);
            TextureSampler.SetFiltering(_Filtering);
            TextureSampler.SetWrapping(_Wrapping);
            FrameSampler.SetTexture(FramebufferAttachment.ColorAttachment0, TextureSampler);
        }
    }

    private Size _Size;
    public Size Size
    {
        get => _Size;
        set
        {
            _Size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            TextureMSCapture.ToAllocate(_InternalFormat, _Size.Width, _Size.Height, _NumSamples, true);
            FrameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, TextureMSCapture);

            RenderCapture.StorageMultisampler(RenderbufferStorage.Depth24Stencil8, _Size.Width, _Size.Height, _NumSamples);
            FrameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, RenderCapture);

            // Texture2D result
            TextureSampler.ToAllocate(_InternalFormat, _Size.Width, _Size.Height);
            TextureSampler.SetFiltering(_Filtering);
            TextureSampler.SetWrapping(_Wrapping);
            FrameSampler.SetTexture(FramebufferAttachment.ColorAttachment0, TextureSampler);
        }
    }
    private TextureFiltering _Filtering = TextureFiltering.Nearest;
    public TextureFiltering Filtering
    {
        get => _Filtering;
        set
        {
            _Filtering = value;
            TextureSampler.SetFiltering(_Filtering);
        }
    }
    private Texture2DWrapping _Wrapping = Texture2DWrapping.ClampToEdge;
    public Texture2DWrapping Wrapping
    {
        get => _Wrapping;
        set
        {
            _Wrapping = value;
            TextureSampler.SetWrapping(_Wrapping);
        }
    }
    private int _NumSamples = 4;
    public int NumSamples
    {
        get => TextureMSCapture.Samples;
        set
        {
            _NumSamples = Math.Clamp(value, 1, 32);

            TextureMSCapture.ToAllocate(_InternalFormat, _Size.Width, _Size.Height, _NumSamples, true);
            FrameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, TextureMSCapture);

            RenderCapture.StorageMultisampler(RenderbufferStorage.Depth24Stencil8, _Size.Width, _Size.Height, _NumSamples);
            FrameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, RenderCapture);
        }
    }
    public ITexture2D FrameResult
    {
        get
        {
            FrameCapture.Blit(FrameSampler, TextureSampler.Width, TextureSampler.Height, BlitFramebufferFilter.Nearest, ClearBufferMask.ColorBufferBit);
            return TextureSampler;
        }
    }
    public void BindToDrawing()
    {
        FrameCapture.Bind();
    }
    public void BindToDrawingAndClear(ClearBufferMask ClearBufferMask)
    {
        FrameCapture.BindAndClear(ClearBufferMask);
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
            TextureMSCapture.Dispose();
            FrameCapture.Dispose();
            RenderCapture.Dispose();

            FrameSampler.Dispose();
            TextureSampler.Dispose();
        }
    }
}
