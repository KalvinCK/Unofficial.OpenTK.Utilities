using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;
using OpenTK.Mathematics;
using System.Drawing;

namespace OpenTK.Utilities.Rasterization;

public class Raster2DDepth : IDisposable
{
    private readonly DepthInternalFormat InternalFormat;
    private readonly FrameBufferObject FrameBufferDepth;
    private readonly Texture2D Texture;
    private readonly SamplerObject SamplerObject;

    public ITexture2D TextureCubemapDepth => Texture;
    public ISamplerObject SamplerDepth => SamplerObject;
    public long TextureBindlessHandler => Texture.BindlessHandler;
    public long SamplerBindlessHandler => SamplerObject.BindlessHandler;

    public Raster2DDepth(Size initSize, DepthInternalFormat DepthInternalFormat)
    {
        FrameBufferDepth = new FrameBufferObject();
        Texture = new Texture2D();

        SamplerObject = new SamplerObject();
        SamplerObject.SetWrapping2D(_SamplerWrapping);
        SamplerObject.SetFiltering(_SamplerFiltering);
        SamplerObject.SetSamplerIntParameter(SamplerParameterName.TextureCompareMode, TextureCompareMode.CompareRToTexture);
        SamplerObject.SetSamplerIntParameter(SamplerParameterName.TextureCompareFunc, All.Less);

        InternalFormat = DepthInternalFormat;
        Size = initSize;
    }
    private Size _Size;
    public Size Size
    {
        get => _Size;
        set
        {
            _Size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            Texture.ToAllocate((SizedInternalFormat)InternalFormat, _Size.Width, _Size.Height);
            Texture.SetFiltering(_TextureFiltering);
            Texture.SetWrapping(_TextureWrapping);
            Texture.SetBorderColor(Vector4.One);
            SamplerObject.AttachTexture(Texture);

            FrameBufferDepth.SetTexture(FramebufferAttachment.DepthAttachment, Texture);
            FrameBufferDepth.ClearBuffer(ClearBuffer.Depth, 0, 1.0f);
        }
    }
    private TextureFiltering _TextureFiltering = TextureFiltering.Nearest;
    public TextureFiltering TextureFiltering
    {
        get => _TextureFiltering;
        set
        {
            _TextureFiltering = value;
            Texture.SetFiltering(_TextureFiltering);
        }
    }
    private Texture2DWrapping _TextureWrapping = Texture2DWrapping.ClampToEdge;
    public Texture2DWrapping TextureWrapping
    {
        get => _TextureWrapping;
        set
        {
            _TextureWrapping = value;
            Texture.SetWrapping(_TextureWrapping);
        }
    }

    private TextureFiltering _SamplerFiltering = TextureFiltering.Linear;
    public TextureFiltering SamplerFiltering
    {
        get => _SamplerFiltering;
        set
        {
            _SamplerFiltering = value;
            SamplerObject.SetFiltering(_SamplerFiltering);
        }
    }
    private Texture2DWrapping _SamplerWrapping = Texture2DWrapping.ClampToEdge;
    public Texture2DWrapping SampleWrapping
    {
        get => _SamplerWrapping;
        set
        {
            _SamplerWrapping = value;
            SamplerObject.SetWrapping2D(_SamplerWrapping);
        }
    }
    public void BindToDrawing()
    {
        FrameBufferDepth.Bind();
    }
    public void BindToDrawingAndClear(ClearBufferMask ClearBufferMask)
    {
        FrameBufferDepth.BindAndClear(ClearBufferMask);
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
            FrameBufferDepth.Dispose();
            Texture.Dispose();
        }
    }
}
