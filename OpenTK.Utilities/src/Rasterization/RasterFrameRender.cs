﻿using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Rasterization;

public class RasterFrameRender : IDisposable
{
    private readonly FrameBufferObject frameCapture;
    private readonly RenderBufferObject renderCapture;
    private readonly Texture2D textureCapture;
    private SizedInternalFormat internalFormat;
    private Size size;
    private TextureFiltering textureFiltering = TextureFiltering.Nearest;
    private Texture2DWrapping textureWrapping = Texture2DWrapping.ClampToEdge;

    public RasterFrameRender(Size initSize, SizedInternalFormat internalFormat = SizedInternalFormat.Rgba16f)
    {
        this.frameCapture = new FrameBufferObject();
        this.renderCapture = new RenderBufferObject();
        this.textureCapture = new Texture2D();

        this.internalFormat = internalFormat;
        this.Size = initSize;
    }

    public ITexture2D FrameResult => this.textureCapture;

    public SizedInternalFormat InternalFormat
    {
        get => this.internalFormat;
        set
        {
            this.internalFormat = value;

            this.textureCapture.ToAllocate(this.internalFormat, this.size.Width, this.size.Height);
            this.textureCapture.Filtering = this.textureFiltering;
            this.textureCapture.Wrapping = this.textureWrapping;

            this.frameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureCapture);
        }
    }

    public Size Size
    {
        get => this.size;
        set
        {
            this.size = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));

            this.renderCapture.Storage(RenderbufferStorage.Depth24Stencil8, this.size.Width, this.size.Height);

            this.textureCapture.ToAllocate(this.internalFormat, this.size.Width, this.size.Height);
            this.textureCapture.Filtering = this.textureFiltering;
            this.textureCapture.Wrapping = this.textureWrapping;

            this.frameCapture.SetRenderBuffer(FramebufferAttachment.DepthStencilAttachment, this.renderCapture);
            this.frameCapture.SetTexture(FramebufferAttachment.ColorAttachment0, this.textureCapture);
        }
    }

    public TextureFiltering Filtering
    {
        get => this.textureFiltering;
        set
        {
            this.textureFiltering = value;
            this.textureCapture.Filtering = this.textureFiltering;
        }
    }

    public Texture2DWrapping Wrapping
    {
        get => this.textureWrapping;
        set
        {
            this.textureWrapping = value;
            this.textureCapture.Wrapping = this.textureWrapping;
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
            this.frameCapture.Dispose();
            this.renderCapture.Dispose();
            this.textureCapture.Dispose();
        }
    }
}
