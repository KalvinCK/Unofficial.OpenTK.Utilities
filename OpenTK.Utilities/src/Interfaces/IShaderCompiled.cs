using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public  interface IShaderCompiled : IBuffer
{
    public ShaderType Type { get; }
    public string Name { get; }
}
