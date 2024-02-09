using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;
using System.Drawing;

namespace OpenTK.Utilities.Rasterization;

public class RasterFrameRender : IDisposable
{
    private readonly FrameBufferObject FrameCapture;
    private readonly RenderBufferObject RenderCapture;
    private readonly Texture2D TextureCapture;
    public ITexture2D FrameResult => TextureCapture;
    public RasterFrameRender(Size initSize, SizedInternalFormat internalFormat = SizedInternalFormat.Rgba16f)
    {
        FrameCapture = new FrameBufferObject();
        RenderCapture = new RenderBufferObject();
        TextureCapture = new Texture2D();

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

            TextureCapture.ToAllocate(_InternalFormat, _Size.Width, _Size.Height);
            TextureCapture.SetFiltering(_Filtering);
            TextureCapture.SetWrapping(_Wrapping);

            FrameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, TextureCapture);
        }
    }

    private Size _Size;
    public Size Size
    {
        get => _Size;
        set
        {
            _Size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            RenderCapture.Storage(RenderbufferStorage.Depth24Stencil8, _Size.Width, _Size.Height);

            TextureCapture.ToAllocate(_InternalFormat, _Size.Width, _Size.Height);
            TextureCapture.SetFiltering(_Filtering);
            TextureCapture.SetWrapping(_Wrapping);

            FrameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, RenderCapture);
            FrameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, TextureCapture);
        }
    }
    private TextureFiltering _Filtering = TextureFiltering.Nearest;
    public TextureFiltering Filtering
    {
        get => _Filtering;
        set
        {
            _Filtering = value;
            TextureCapture.SetFiltering(_Filtering);
        }
    }
    private Texture2DWrapping _Wrapping = Texture2DWrapping.ClampToEdge;
    public Texture2DWrapping Wrapping
    {
        get => _Wrapping;
        set
        {
            _Wrapping = value;
            TextureCapture.SetWrapping(_Wrapping);
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
            FrameCapture.Dispose();
            RenderCapture.Dispose();
            TextureCapture.Dispose();
        }
    }
}
