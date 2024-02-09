using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

public interface IFrameBufferObject : IBuffer
{
    public static int BufferBindedInContext { get; internal set; }
    public FramebufferStatus GetStatus();
    public void Bind(FramebufferTarget framebufferTarget = FramebufferTarget.Framebuffer);
    public void BindAndClear(ClearBufferMask ClearBufferMask, FramebufferTarget FramebufferTarget = FramebufferTarget.Framebuffer);
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, float clearValue);
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, uint clearValue);
    public void ClearBuffer(ClearBuffer clearBuffer, int drawBuffer, int clearValue);
    public void Clear(ClearBufferMask clearBufferMask);
    public void DrawTextureColor<TTexture2D>(
        TTexture2D drawTexture,
        BlitFramebufferFilter BlitFramebufferFilter = BlitFramebufferFilter.Nearest,
        ClearBufferMask ClearBufferMask = ClearBufferMask.ColorBufferBit)
        where TTexture2D : ITexture2D;

    public static readonly IFrameBufferObject Default = new FrameBufferObject(0);
}
