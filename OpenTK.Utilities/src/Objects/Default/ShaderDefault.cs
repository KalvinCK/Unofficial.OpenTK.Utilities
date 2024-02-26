using System.Collections.ObjectModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public readonly struct ShaderDefault : IReadOnlyShaderObject
{
    #region Props
    public int BufferID { get;  }

    public ReadOnlyDictionary<string, int> ActivesUniforms { get; }

    public ReadOnlyDictionary<string, int> ActivesAttributes { get; }

    public IReadOnlyList<ShaderType> Composion { get; }

    public bool Separable { get; }

    public string Label
    {
        get
        {
            GL.GetObjectLabel(ObjectLabelIdentifier.Program, this.BufferID, 64, out int _, out string label);
            return label;
        }

        set
        {
            GL.ObjectLabel(ObjectLabelIdentifier.Program, this.BufferID, value.Length, value);
        }
    }

    #endregion

    #region ###
    public void Bind()
    {
        GL.UseProgram(this.BufferID);
    }

    public void DispatchCompute(uint groupsX = 1, uint groupsY = 1, uint groupsZ = 1)
    {
        this.Bind();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
    }

    public void DispatchCompute(int groupsX = 1, int groupsY = 1, int groupsZ = 1)
    {
        this.Bind();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
    }

    public void DispatchCompute(DispatchCommand dispatchCommand)
    {
        this.Bind();
        GL.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
    }

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
        where TTexture : IReadOnlyTexture
    {
        texture.BindToImageUnit(unit, level, layered, layer, TextureAccess);
        this.DispatchCompute(groupsX, groupsY, groupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);
    }

    public void DispatchCompute<TTexture>(
        TTexture texture,
        DispatchCommand dispatchCommand,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly)
        where TTexture : IReadOnlyTexture
    {
        texture.BindToImageUnit(unit, level, layered, layer, TextureAccess);
        this.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);
    }

    #endregion
}
