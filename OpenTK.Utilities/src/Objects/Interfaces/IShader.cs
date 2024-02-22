using System.Collections.ObjectModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public interface IShader : IBuffer
{
    public static int BufferBindedInContext { get; internal set; }

    public ReadOnlyDictionary<string, int> ActivesUniforms { get; }

    public IReadOnlyList<ShaderType> Composion { get; }

    public bool Separable { get; }

    public void Use();

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
        where TTexture : ITexture;

    public void DispatchCompute<TTexture>(
        TTexture texture,
        DispatchCommand dispatchcommand,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly)
        where TTexture : ITexture;

    public static void ClearContext()
    {
        GL.UseProgram(0);
        BufferBindedInContext = 0;
    }
}
