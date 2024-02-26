using System.Collections.ObjectModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public interface IReadOnlyShaderObject : IReadOnlyBuffer
{
    public ReadOnlyDictionary<string, int> ActivesUniforms { get; }

    public IReadOnlyList<ShaderType> Composion { get; }

    public bool Separable { get; }

    public string Label { get; }

    /// <summary>
    /// Link to context.
    /// </summary>
    /// <remarks>
    /// He does: <see cref="GL.UseProgram(int)"/>.
    /// </remarks>
    public void Bind();

    public void DispatchCompute(uint groupsX = 1, uint groupsY = 1, uint groupsZ = 1);

    public void DispatchCompute(int groupsX = 1, int groupsY = 1, int groupsZ = 1);

    public void DispatchCompute(DispatchCommand dispatchCommand);

    public void DispatchCompute<TTexture>(
        TTexture texture,
        int groupsX = 1,
        int groupsY = 1,
        int groupsZ = 1,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly)
        where TTexture : IReadOnlyTexture;

    public void DispatchCompute<TTexture>(
        TTexture texture,
        DispatchCommand dispatchcommand,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly)
        where TTexture : IReadOnlyTexture;
}
