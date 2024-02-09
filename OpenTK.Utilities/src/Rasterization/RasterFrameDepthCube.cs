using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;
using System.Drawing;

namespace OpenTK.Utilities.Rasterization;

public class RasterFrameDepthCube : IDisposable
{
    private readonly DepthInternalFormat InternalFormat;
    private readonly FrameBufferObject FrameBufferDepth;
    private readonly TextureCubeMap TextureDepth;
    private readonly SamplerObject SamplerObjectDepth;

    public ITexture3D TextureCubemapDepth => TextureDepth;
    public ISamplerObject SamplerDepth => SamplerObjectDepth;
    public long TextureBindlessHandler => TextureDepth.BindlessHandler;
    public long SamplerBindlessHandler => SamplerObjectDepth.BindlessHandler;

    public RasterFrameDepthCube(Size initSize, DepthInternalFormat DepthInternalFormat)
    {
        FrameBufferDepth = new FrameBufferObject();
        TextureDepth = new TextureCubeMap();

        SamplerObjectDepth = new SamplerObject();
        SamplerObjectDepth.SetWrapping3D(_SamplerWrapping);
        SamplerObjectDepth.SetFiltering(_SamplerFiltering);
        SamplerObjectDepth.SetSamplerIntParameter(SamplerParameterName.TextureCompareMode, TextureCompareMode.CompareRefToTexture);
        SamplerObjectDepth.SetSamplerIntParameter(SamplerParameterName.TextureCompareFunc, All.Less);

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

            TextureDepth.ToAllocate((SizedInternalFormat)InternalFormat, _Size.Width, _Size.Height);
            TextureDepth.SetFiltering(_TextureFiltering);
            TextureDepth.SetWrapping(_TextureWrapping);
            SamplerObjectDepth.AttachTexture(TextureDepth);

            FrameBufferDepth.SetTexture(FramebufferAttachment.DepthAttachment, TextureDepth);
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
            TextureDepth.SetFiltering(_TextureFiltering);
        }
    }
    private Texture3DWrapping _TextureWrapping = Texture3DWrapping.ClampToEdge;
    public Texture3DWrapping TextureWrapping
    {
        get => _TextureWrapping;
        set
        {
            _TextureWrapping = value;
            TextureDepth.SetWrapping(_TextureWrapping);
        }
    }

    private TextureFiltering _SamplerFiltering = TextureFiltering.Linear;
    public TextureFiltering SamplerFiltering
    {
        get => _SamplerFiltering;
        set
        {
            _SamplerFiltering = value;
            SamplerObjectDepth.SetFiltering(_SamplerFiltering);
        }
    }
    private Texture3DWrapping _SamplerWrapping = Texture3DWrapping.ClampToEdge;
    public Texture3DWrapping SampleWrapping
    {
        get => _SamplerWrapping;
        set
        {
            _SamplerWrapping = value;
            SamplerObjectDepth.SetWrapping3D(_SamplerWrapping);
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
            TextureDepth.Dispose();
            SamplerObjectDepth.Dispose();
        }
    }
}
