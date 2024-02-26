using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyFramebufferObject : IReadOnlyBuffer
{
    public FramebufferStatus GetStatus(FramebufferTarget framebufferTarget);

    public void Blit(in FramebufferObject drawFrameBuffer, int width, int height, BlitFramebufferFilter blitFramebufferFilter, ClearBufferMask bufferMask, int srcX = 0, int srcY = 0, int dstX = 0, int dstY = 0);

    public void Bind(FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer);

    public void BindAndClear(ClearBufferMask ClearBufferMask, FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer);

    public void ClearBuffer(ClearBuffer ClearBuffer, int drawBuffer, float clearValue);

    public void ClearBuffer(ClearBuffer ClearBuffer, int drawBuffer, uint clearValue);

    public void ClearBuffer(ClearBuffer ClearBuffer, int drawBuffer, int clearValue);

    public void Clear(ClearBufferMask ClearBufferMask);

    public Tx2D ExtractTextureColor<Tx2D>(Size sizeTex)
        where Tx2D : IReadOnlyTexture2D;

    public void DrawTextureColor<TTexture2D>(
        ref TTexture2D drawTexture,
        BlitFramebufferFilter BlitFramebufferFilter = BlitFramebufferFilter.Nearest,
        ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit)
        where TTexture2D : IReadOnlyTexture2D;

    internal static int CreateBuffer()
    {
        GL.CreateFramebuffers(1, out int buffer);
        return buffer;
    }
}
