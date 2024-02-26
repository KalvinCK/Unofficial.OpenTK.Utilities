using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public class SamplerObject : IReadOnlySamplerObject, IDisposable
{
    public SamplerObject()
    {
        this.BufferID = IReadOnlySamplerObject.CreateBuffer();
    }

    public int BufferID { get; private set; }

    public long BindlessHandler { get; private set; }

    public void Bind(int unit)
    {
        GL.BindSampler(unit, this.BufferID);
    }

    #region Sets
    public void SetSamplerIntParameter<TEnum>(SamplerParameterName samplerParameterName, TEnum enumParam)
        where TEnum : Enum
    {
        GL.SamplerParameter(this.BufferID, samplerParameterName, (int)(object)enumParam);
    }

    public void SetSamplerIntParameters<TEnum>(SamplerParameterName samplerParameterName, params TEnum[] enumParams)
        where TEnum : Enum
    {
        int[] intParams = enumParams.Select(e => (int)(object)e).ToArray();
        GL.SamplerParameter(this.BufferID, samplerParameterName, intParams);
    }

    public void SetSamplerFloatParameter<TEnum>(SamplerParameterName samplerParameterName, TEnum enumParam)
        where TEnum : Enum
    {
        GL.SamplerParameter(this.BufferID, samplerParameterName, (float)(object)enumParam);
    }

    public void SetSamplerFloatParameters<TEnum>(SamplerParameterName samplerParameterName, params TEnum[] enumParams)
        where TEnum : Enum
    {
        float[] floatParams = enumParams.Select(e => (float)(object)e).ToArray();
        GL.SamplerParameter(this.BufferID, samplerParameterName, floatParams);
    }

    public void SetFiltering(TextureFiltering filtering)
    {
        this.SetSamplerIntParameter(SamplerParameterName.TextureMinFilter, filtering.MinFilter);
        this.SetSamplerIntParameter(SamplerParameterName.TextureMagFilter, filtering.MagFilter);
    }

    public void SetWrapping1D(TextureWrapMode wrapModeS)
    {
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapModeS);
    }

    public void SetWrapping2D(Texture2DWrapping wrapping)
    {
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapping.WrapModeS);
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapT, wrapping.WrapModeT);
    }

    public void SetWrapping3D(Texture3DWrapping wrapping)
    {
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapS, wrapping.WrapModeS);
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapT, wrapping.WrapModeT);
        this.SetSamplerIntParameter(SamplerParameterName.TextureWrapR, wrapping.WrapModeR);
    }
    #endregion

    #region extension

    /// <summary>
    /// #extension: GL_ARB_bindless_texture : required.
    /// </summary>
    /// <typeparam name="TTexture">texture object.</typeparam>
    /// <param name="texture">texture to attach.</param>
    public void AttachTexture<TTexture>(TTexture texture)
        where TTexture : IReadOnlyTexture
    {
        this.BindlessHandler = GL.Arb.GetTextureSamplerHandle(texture.BufferID, this.BufferID);
        GL.Arb.MakeTextureHandleResident(this.BindlessHandler);
    }
    #endregion

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (this.BindlessHandler is not 0)
            {
                GL.Arb.MakeTextureHandleNonResident(this.BindlessHandler);
            }

            GL.DeleteSampler(this.BufferID);
            this.BindlessHandler = 0;
            this.BufferID = 0;
        }
    }
}
