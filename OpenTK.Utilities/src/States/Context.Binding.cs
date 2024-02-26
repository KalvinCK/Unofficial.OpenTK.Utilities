using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

public partial class Context
{
    public static class Binding
    {
        public static IReadOnlyBuffer Texture1DBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding1D));

        public static IReadOnlyBuffer Texture1DArrayBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding1DArray));

        public static IReadOnlyBuffer Texture2DBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding2D));

        public static IReadOnlyBuffer Texture2DArrayBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding2DArray));

        public static IReadOnlyBuffer TextureRectangleBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBindingRectangle));

        public static IReadOnlyBuffer Texture3DBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding3D));

        public static IReadOnlyBuffer TextureCubeMapBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBindingCubeMap));

        public static IReadOnlyBuffer Texture2DMultisampleBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding2DMultisample));

        public static IReadOnlyBuffer Texture2DMultisampleArrayBinding => new BufferReadOnly(GL.GetInteger(GetPName.TextureBinding2DMultisampleArray));

        public static IReadOnlyBuffer PipelineBinding => new BufferReadOnly(GL.GetInteger(GetPName.ProgramPipelineBinding));

        public static IReadOnlyBuffer ShaderBinding => new BufferReadOnly(GL.GetInteger(GetPName.CurrentProgram));

        public static IReadOnlyBuffer VertexArrayBinding => new BufferReadOnly(GL.GetInteger(GetPName.VertexArrayBinding));

        public static IReadOnlyBuffer BufferBinding => new BufferReadOnly(GL.GetInteger((GetPName)All.BufferBinding));

        public static IReadOnlyBuffer BufferVertexBinding => new BufferReadOnly(GL.GetInteger(GetPName.VertexArrayBufferBinding));

        public static IReadOnlyBuffer BufferElementBinding => new BufferReadOnly(GL.GetInteger(GetPName.ElementArrayBufferBinding));

        public static IReadOnlyBuffer FramebufferBinding => new BufferReadOnly(GL.GetInteger(GetPName.FramebufferBinding));

        public static IReadOnlyBuffer DrawFramebufferBinding => new BufferReadOnly(GL.GetInteger(GetPName.DrawFramebufferBinding));

        public static IReadOnlyBuffer ReadFramebufferBinding => new BufferReadOnly(GL.GetInteger(GetPName.ReadFramebufferBinding));

        public static IReadOnlyBuffer RenderbufferBinding => new BufferReadOnly(GL.GetInteger(GetPName.RenderbufferBinding));

        public static IReadOnlyBuffer SamplerBinding => new BufferReadOnly(GL.GetInteger(GetPName.SamplerBinding));

        /// <summary>
        /// Each OpenGL object has a default object used to unbind the state,
        /// with this call everything is reset to the <c>"Default" </c> objects.
        /// </summary>
        public static void ResetContext()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindVertexArray(0);
            GL.BindProgramPipeline(0);
            GL.UseProgram(0);

            foreach (var item in Enum.GetValues(typeof(BufferTarget)))
            {
                GL.BindBuffer((BufferTarget)item, 0);
            }

            foreach (var item in Enum.GetValues(typeof(TextureFormat)))
            {
                GL.BindTexture((TextureTarget)(TextureFormat)item, 0);
            }
        }
    }

    internal class BufferReadOnly(int buffer) : IReadOnlyBuffer
    {
        public int BufferID { get; } = buffer;
    }
}
