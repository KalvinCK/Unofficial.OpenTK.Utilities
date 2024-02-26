using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyShaderSource : IReadOnlyBuffer
{
    public ShaderType Type { get; }
}
