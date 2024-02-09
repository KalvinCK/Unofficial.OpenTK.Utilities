using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class SamplerObject : ISamplerObject, IDisposable
{
    public int BufferID { get; private set; }
    public long BindlessHandler { get; private set; }
    public SamplerObject()
    {
        GL.CreateSamplers(1, out int buffer);
        BufferID = buffer;
    }

    #region State
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (BindlessHandler is not 0) GL.Arb.MakeTextureHandleNonResident(BindlessHandler);
            GL.DeleteSampler(BufferID);
            BindlessHandler = 0;
            BufferID = 0;
        }
    }
    public void Bind(int unit)
    {
        GL.BindSampler(unit, BufferID);
    }
    
    #endregion

    #region Sets
    public void SetSamplerParamter(SamplerParameterName samplerParameterName, int param)
    {
        GL.SamplerParameter(BufferID, samplerParameterName, param);
    }
    public void SetSamplerParamter(SamplerParameterName samplerParameterName, int[] param)
    {
        GL.SamplerParameter(BufferID, samplerParameterName, param);
    }

    public void SetSamplerParamter(SamplerParameterName samplerParameterName, float param)
    {
        GL.SamplerParameter(BufferID, samplerParameterName, param);
    }
    public void SetSamplerParamter(SamplerParameterName samplerParameterName, float[] param)
    {
        GL.SamplerParameter(BufferID, samplerParameterName, param);
    }

    public void SetSamplerIntParameter<TEnum>(SamplerParameterName samplerParameterName, TEnum enumParam) where TEnum : Enum
    {
        GL.SamplerParameter(BufferID, samplerParameterName, (int)(object)enumParam);
    }
    public void SetSamplerIntParameters<TEnum>(SamplerParameterName samplerParameterName, params TEnum[] enumParams) where TEnum : Enum
    {
        int[] intParams = enumParams.Select(e => (int)(object)e).ToArray();
        GL.SamplerParameter(BufferID, samplerParameterName, intParams);
    }
    public void SetSamplerFloatParameter<TEnum>(SamplerParameterName samplerParameterName, TEnum enumParam) where TEnum : Enum
    {
        GL.SamplerParameter(BufferID, samplerParameterName, (float)(object)enumParam);
    }
    public void SetSamplerFloatParameters<TEnum>(SamplerParameterName samplerParameterName, params TEnum[] enumParams) where TEnum : Enum
    {
        float[] floatParams = enumParams.Select(e => (float)(object)e).ToArray();
        GL.SamplerParameter(BufferID, samplerParameterName, floatParams);
    }
    public void SetFiltering(TextureFiltering filtering)
    {
        SetSamplerIntParameter(SamplerParameterName.TextureMinFilter, filtering.MinFilter);
        SetSamplerIntParameter(SamplerParameterName.TextureMagFilter, filtering.MagFilter);
    }
    public void SetWrapping1D(TextureWrapMode wrapModeS)
    {
        SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapModeS);
    }
    public void SetWrapping2D(Texture2DWrapping wrapping)
    {
        SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapping.WrapModeS);
        SetSamplerIntParameter(SamplerParameterName.TextureWrapT, wrapping.WrapModeT);
    }
    public void SetWrapping3D(Texture3DWrapping wrapping)
    {
        SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapping.WrapModeS);
        SetSamplerIntParameter(SamplerParameterName.TextureWrapT, wrapping.WrapModeT);
        SetSamplerIntParameter(SamplerParameterName.TextureWrapR, wrapping.WrapModeR);
    }
    #endregion

    #region extension
    /// <summary>
    /// #extension: GL_ARB_bindless_texture : required
    /// </summary>
    public void AttachTexture<TTexture>(TTexture texture) where TTexture : ITexture
    {
        BindlessHandler = GL.Arb.GetTextureSamplerHandle(texture.BufferID, BufferID);
        GL.Arb.MakeTextureHandleResident(BindlessHandler);
    }
    #endregion
}
