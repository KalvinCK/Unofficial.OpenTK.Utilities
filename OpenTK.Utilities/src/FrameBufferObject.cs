using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

public class FrameBufferObject : IFrameBufferObject, IDisposable
{
    public int BufferID { get; private set; }
    public FrameBufferObject()
    {
        GL.CreateFramebuffers(1, out int buffer);
        BufferID = buffer;
    }
    internal FrameBufferObject(int buffer)
    {
        BufferID = buffer;
    }
    public void Bind(FramebufferTarget framebufferTarget = FramebufferTarget.Framebuffer)
    {
        GL.BindFramebuffer(framebufferTarget, BufferID);
        IFrameBufferObject.BufferBindedInContext = BufferID;
    }
    public FramebufferStatus GetStatus()
    {
        return GL.CheckNamedFramebufferStatus(BufferID, FramebufferTarget.Framebuffer);
    }
    public void BindAndClear(ClearBufferMask ClearBufferMask, FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer)
    {
        Bind(FramebufferTarget);
        Clear(ClearBufferMask);
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if(disposing)
        {
            GL.DeleteFramebuffer(BufferID);
            BufferID = 0;
        }
    }
    public void SetTexture<TTexture>(FramebufferAttachment framebufferAttachment, TTexture Texture, int level = 0) where TTexture : ITexture
    {
        GL.NamedFramebufferTexture(BufferID, framebufferAttachment, Texture.BufferID, level);
    }
    public void SetTextureLayer<TTexture>(FramebufferAttachment framebufferAttachment, TTexture Texture, int layer, int level = 0) where TTexture : ITexture
    {
        GL.NamedFramebufferTextureLayer(BufferID, framebufferAttachment, Texture.BufferID, level, layer);
    }
    public void SetTextureCubeMap<TTexture>(FramebufferAttachment framebufferAttachment, TTexture Texture, CubeMapLayer layer, int level = 0) where TTexture : ITexture
    {
        GL.NamedFramebufferTextureLayer(BufferID, framebufferAttachment, Texture.BufferID, level, (int)layer);
    }
    public void SetRenderBuffer(FramebufferAttachment attachment, RenderBufferObject renderBuffer)
    {
        GL.NamedFramebufferRenderbuffer(BufferID, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.BufferID);
    }
    public void SetParamater(FramebufferDefaultParameter framebufferDefaultParameter, int param)
    {
        GL.NamedFramebufferParameter(BufferID, framebufferDefaultParameter, param);
    }
    public void SetDrawBuffer(DrawBufferMode drawBufferMode)
    {
        GL.NamedFramebufferDrawBuffer(BufferID, drawBufferMode);
    }
    public unsafe void SetDrawBuffers(params DrawBuffersEnum[] drawBuffersEnums)
    {
        fixed (DrawBuffersEnum* ptr = drawBuffersEnums)
        {
            GL.NamedFramebufferDrawBuffers(BufferID, drawBuffersEnums.Length, ptr);
        }
    }
    public void SetReadBuffer(ReadBufferMode readBufferMode)
    {
        GL.NamedFramebufferReadBuffer(BufferID, readBufferMode);
    }
    public void Blit(in FrameBufferObject drawFrameBuffer, int width, int height, BlitFramebufferFilter blitFramebufferFilter, ClearBufferMask bufferMask, int srcX = 0, int srcY = 0, int dstX = 0, int dstY = 0)
    {
        GL.BlitNamedFramebuffer(BufferID, drawFrameBuffer.BufferID, srcX, srcY, width, height, dstX, dstY, width, height, bufferMask, blitFramebufferFilter);
    }
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, float clearValue)
    {
        GL.ClearNamedFramebuffer(BufferID, clearBuffer, drawBuffer, ref clearValue);
    }
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, uint clearValue)
    {
        GL.ClearNamedFramebuffer((uint)BufferID, clearBuffer, drawBuffer, ref clearValue);
    }
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, int clearValue)
    {
        GL.ClearNamedFramebuffer(BufferID, clearBuffer, drawBuffer, ref clearValue);
    }
    public void Clear(ClearBufferMask clearBufferMask)
    {
        GL.Clear(clearBufferMask);
    }
    
    public void DrawTextureColor<TTexture2D>(
        TTexture2D drawTexture,
        BlitFramebufferFilter BlitFramebufferFilter = BlitFramebufferFilter.Nearest,
        ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit) where TTexture2D : ITexture2D
    {
        using var drawFrame = new FrameBufferObject();
        drawFrame.SetTexture(FramebufferAttachment.ColorAttachment0, drawTexture);

        Blit(drawFrame, drawTexture.Width, drawTexture.Height, BlitFramebufferFilter, ClearBufferMask);
    }
}