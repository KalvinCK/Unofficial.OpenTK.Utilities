using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public readonly struct FramebufferDefault : IReadOnlyFramebufferObject
{
    public int BufferID { get; }

    public FramebufferStatus GetStatus(FramebufferTarget framebufferTarget)
    {
        return GL.CheckNamedFramebufferStatus(this.BufferID, framebufferTarget);
    }

    public void Blit(in FramebufferObject drawFrameBuffer, int width, int height, BlitFramebufferFilter blitFramebufferFilter, ClearBufferMask bufferMask, int srcX = 0, int srcY = 0, int dstX = 0, int dstY = 0)
    {
        GL.BlitNamedFramebuffer(this.BufferID, drawFrameBuffer.BufferID, srcX, srcY, width, height, dstX, dstY, width, height, bufferMask, blitFramebufferFilter);
    }

    public void Bind(FramebufferTarget FramebufferTarget)
    {
        GL.BindFramebuffer(FramebufferTarget, this.BufferID);
    }

    public void BindAndClear(ClearBufferMask ClearBufferMask, FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer)
    {
        this.Bind(FramebufferTarget);
        this.Clear(ClearBufferMask);
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

    public Tex2D ExtractTextureColor<Tex2D>(Size sizeTex)
        where Tex2D : IReadOnlyTexture2D
    {
        if (typeof(Tex2D) == typeof(TextureRectangle))
        {
            var texture = new TextureRectangle(TextureFormat.Rgb8, sizeTex.Width, sizeTex.Height);
            this.DrawTextureColor(ref texture);
            return (Tex2D)Convert.ChangeType(texture, typeof(Tex2D));
        }
        else
        {
            var texture = new Texture2D(TextureFormat.Rgb8, sizeTex.Width, sizeTex.Height);
            this.DrawTextureColor(ref texture);
            return (Tex2D)Convert.ChangeType(texture, typeof(Tex2D));
        }
    }

    public void DrawTextureColor<TTexture2D>(
        ref TTexture2D drawTexture,
        BlitFramebufferFilter BlitFramebufferFilter = BlitFramebufferFilter.Nearest,
        ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit)
        where TTexture2D : IReadOnlyTexture2D
    {
        using var drawFrame = new FramebufferObject();
        drawFrame.SetTexture(FramebufferAttachment.ColorAttachment0, drawTexture);

        this.Blit(drawFrame, drawTexture.Width, drawTexture.Height, BlitFramebufferFilter, ClearBufferMask);
    }
}
