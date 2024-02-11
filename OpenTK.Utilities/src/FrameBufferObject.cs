using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

public class FrameBufferObject : IFrameBufferObject, IDisposable
{
    public FrameBufferObject()
    {
        this.BufferID = IFrameBufferObject.CreateBuffer();
    }

    internal FrameBufferObject(int buffer)
    {
        this.BufferID = buffer;
    }

    public int BufferID { get; private set; }

    public void Bind(FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer)
    {
        GL.BindFramebuffer(FramebufferTarget, this.BufferID);
        IFrameBufferObject.BufferBindedInContext = this.BufferID;
    }

    public FramebufferStatus GetStatus()
    {
        return GL.CheckNamedFramebufferStatus(this.BufferID, FramebufferTarget.Framebuffer);
    }

    public void BindAndClear(ClearBufferMask ClearBufferMask, FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer)
    {
        this.Bind(FramebufferTarget);
        this.Clear(ClearBufferMask);
    }

    public void SetTexture<TTexture>(FramebufferAttachment FramebufferAttachment, TTexture texture, int level = 0)
        where TTexture : ITexture
    {
        GL.NamedFramebufferTexture(this.BufferID, FramebufferAttachment, texture.BufferID, level);
    }

    public void SetTextureLayer<TTexture>(FramebufferAttachment FramebufferAttachment, TTexture Texture, int layer, int level = 0)
        where TTexture : ITexture
    {
        GL.NamedFramebufferTextureLayer(this.BufferID, FramebufferAttachment, Texture.BufferID, level, layer);
    }

    public void SetTextureCubeMap<TTexture>(FramebufferAttachment FramebufferAttachment, TTexture Texture, CubeMapLayer layer, int level = 0)
        where TTexture : ITexture
    {
        GL.NamedFramebufferTextureLayer(this.BufferID, FramebufferAttachment, Texture.BufferID, level, (int)layer);
    }

    public void SetRenderBuffer(FramebufferAttachment attachment, RenderBufferObject renderBuffer)
    {
        GL.NamedFramebufferRenderbuffer(this.BufferID, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.BufferID);
    }

    public void SetParamater(FramebufferDefaultParameter FramebufferDefaultParameter, int param)
    {
        GL.NamedFramebufferParameter(this.BufferID, FramebufferDefaultParameter, param);
    }

    public void SetDrawBuffer(DrawBufferMode drawBufferMode)
    {
        GL.NamedFramebufferDrawBuffer(this.BufferID, drawBufferMode);
    }

    public unsafe void SetDrawBuffers(params DrawBuffersEnum[] drawBuffersEnums)
    {
        fixed (DrawBuffersEnum* ptr = drawBuffersEnums)
        {
            GL.NamedFramebufferDrawBuffers(this.BufferID, drawBuffersEnums.Length, ptr);
        }
    }

    public void SetReadBuffer(ReadBufferMode readBufferMode)
    {
        GL.NamedFramebufferReadBuffer(this.BufferID, readBufferMode);
    }

    public void Blit(in FrameBufferObject drawFrameBuffer, int width, int height, BlitFramebufferFilter blitFramebufferFilter, ClearBufferMask bufferMask, int srcX = 0, int srcY = 0, int dstX = 0, int dstY = 0)
    {
        GL.BlitNamedFramebuffer(this.BufferID, drawFrameBuffer.BufferID, srcX, srcY, width, height, dstX, dstY, width, height, bufferMask, blitFramebufferFilter);
    }

    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, float clearValue)
    {
        GL.ClearNamedFramebuffer(this.BufferID, clearBuffer, drawBuffer, ref clearValue);
    }

    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, uint clearValue)
    {
        GL.ClearNamedFramebuffer((uint)this.BufferID, clearBuffer, drawBuffer, ref clearValue);
    }

    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, int clearValue)
    {
        GL.ClearNamedFramebuffer(this.BufferID, clearBuffer, drawBuffer, ref clearValue);
    }

    public void Clear(ClearBufferMask clearBufferMask)
    {
        GL.Clear(clearBufferMask);
    }

    public void DrawTextureColor<TTexture2D>(
        TTexture2D drawTexture,
        BlitFramebufferFilter BlitFramebufferFilter = BlitFramebufferFilter.Nearest,
        ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit)
        where TTexture2D : ITexture2D
    {
        using var drawFrame = new FrameBufferObject();
        drawFrame.SetTexture(FramebufferAttachment.ColorAttachment0, drawTexture);

        this.Blit(drawFrame, drawTexture.Width, drawTexture.Height, BlitFramebufferFilter, ClearBufferMask);
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
            GL.DeleteFramebuffer(this.BufferID);
            this.BufferID = 0;
        }
    }
}
