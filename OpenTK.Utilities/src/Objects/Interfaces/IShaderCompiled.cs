using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IShaderCompiled : IBuffer
{
    public ShaderType Type { get; }

    public string Name { get; }
}
