using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IShaderSource : IBuffer
{
    public ShaderType Type { get; }
}
