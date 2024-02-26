using OpenTK.Utilities.Objects;

namespace OpenTK.Utilities;

public static partial class Context
{
    public static class Default
    {
        public static readonly FramebufferDefault DefaultFrameBuffer = default;
        public static readonly RenderbufferDefault DefaultRenderBuffer = default;
        public static readonly VertexArrayDefault DefaultVertexArray = default;
        public static readonly ShaderDefault DefaultShader = default;
        public static readonly PipelineDefault DefaultPipeline = default;
    }
}
