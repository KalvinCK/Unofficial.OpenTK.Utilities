using System.Collections.ObjectModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Objects;

public class Shader : IShader, IDisposable
{
    public Shader(params IShaderSource[] shaders)
    {
        this.BufferID = Create(false, shaders);
        this.Composion = shaders.Select(i => i.Type).ToList();
        this.ActivesUniforms = ProcessUniforms(this.BufferID);
        this.ActivesAttributes = ProcessAttributes(this.BufferID);
        this.Separable = false;
    }

    public Shader(bool separable, params IShaderSource[] shaders)
    {
        this.BufferID = Create(separable, shaders);
        this.Composion = shaders.Select(i => i.Type).ToList();
        this.ActivesUniforms = ProcessUniforms(this.BufferID);
        this.ActivesAttributes = ProcessAttributes(this.BufferID);
        this.Separable = separable;
    }

    private Shader(int programID, List<ShaderType> composion, bool separable)
    {
        this.BufferID = programID;
        this.Composion = composion;
        this.Separable = separable;
        this.ActivesUniforms = ProcessUniforms(this.BufferID);
        this.ActivesAttributes = ProcessAttributes(this.BufferID);
    }

    #region Props
    public int BufferID { get; private set; }

    public ReadOnlyDictionary<string, int> ActivesUniforms { get; private set; }

    public ReadOnlyDictionary<string, int> ActivesAttributes { get; private set; }

    public IReadOnlyList<ShaderType> Composion { get; private set; }

    public bool Separable { get; private set; }

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

    /// <summary>
    /// Gets or sets a value indicating whether to stop the program when
    /// uniform and attribute names are incorrect or do not exist in the shader.
    /// </summary>
    public bool EnableExceptions { get; set; }

    #endregion

    public static Shader CreateProgram(params IShaderSource[] shaders)
    {
        return new Shader(Create(false, shaders), shaders.Select(i => i.Type).ToList(), false);
    }

    public static Shader CreateProgramSeparable(params IShaderSource[] shaders)
    {
        return new Shader(Create(true, shaders), shaders.Select(i => i.Type).ToList(), true);
    }

    #region Gets
    public bool ContainsUniform(string name)
    {
        return this.ActivesUniforms.ContainsKey(name);
    }

    public bool ContainsAttribute(string name)
    {
        return this.ActivesAttributes.ContainsKey(name);
    }

    public int GetAttribute(string name)
    {
        bool result = this.ActivesAttributes.TryGetValue(name, out int value);

        if (!result && this.EnableExceptions)
        {
            throw new AttributeNotFoundException(name);
        }

        return result ? value : -1;
    }

    public int GetUniformLocation(string name)
    {
        bool result = this.ActivesUniforms.TryGetValue(name, out int value);

        if (!result && this.EnableExceptions)
        {
            throw new UniformNotFoundException(name);
        }

        return result ? value : -1;
    }

    public int ValidateUniformLocation(int uniformLocation)
    {
        if (this.ActivesUniforms.Values.ToList().IndexOf(uniformLocation) == -1 && this.EnableExceptions)
        {
            throw new UniformNotFoundException($"in location: '{uniformLocation}'");
        }

        return uniformLocation;
    }

    #endregion

    #region Uploads
    public void Uniform(int location, int data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, this.ValidateUniformLocation(location), count, ref data);

    public void Uniform(int location, uint data, int count = 1)
        => GL.ProgramUniform1((uint)this.BufferID, location, count, ref data);

    public void Uniform(int location, float data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, this.ValidateUniformLocation(location), count, ref data);

    public void Uniform(int location, double data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, this.ValidateUniformLocation(location), count, ref data);

    public void Uniform(int location, long data, int count = 1)
        => GL.Arb.ProgramUniformHandle(this.BufferID, this.ValidateUniformLocation(location), count, ref data);

    public void Uniform(int location, bool data)
        => this.Uniform(location, data ? 1 : 0);

    public unsafe void Uniform(int location, in Size data)
        => this.Uniform(location, new Vector2i(data.Width, data.Height));

    public unsafe void Uniform(int location, in Point data)
        => this.Uniform(location, new Vector2i(data.X, data.Y));

    public unsafe void Uniform(int location, in SizeF data)
        => this.Uniform(location, new Vector2(data.Width, data.Height));

    public unsafe void Uniform(int location, in PointF data)
        => this.Uniform(location, new Vector2(data.X, data.Y));

    public void Uniform<TEnum>(int location, TEnum enumData)
        where TEnum : Enum
        => this.Uniform(location, (int)(object)enumData);

    public void Uniform(string name, int data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public void Uniform(string name, uint data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public void Uniform(string name, float data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public void Uniform(string name, double data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public void Uniform(string name, long data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public void Uniform(string name, bool data)
        => this.Uniform(name, data ? 1 : 0, 1);

    public unsafe void Uniform(string name, in Size data)
        => this.Uniform(this.GetUniformLocation(name), data);

    public unsafe void Uniform(string name, in SizeF data)
        => this.Uniform(this.GetUniformLocation(name), data);

    public unsafe void Uniform(string name, in Point data)
        => this.Uniform(this.GetUniformLocation(name), data);

    public unsafe void Uniform(string name, in PointF data)
        => this.Uniform(this.GetUniformLocation(name), new Vector2(data.X, data.Y));

    public void Uniform<TEnum>(string name, TEnum enumData)
        where TEnum : Enum
        => this.Uniform(name, (int)(object)enumData);

    #region SetUniformTK

    #region Sets Int
    public unsafe void Uniform(int location, in Vector2i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform2(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(string name, in Vector2i data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector3i data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector4i data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    #endregion

    #region Sets Float
    public unsafe void Uniform(int location, in Vector2 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform2(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.W)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in Vector2 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector3 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector4 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Quaternion data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Matrix2 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix2x3 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix2x4 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3x2 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3x4 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4x2 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4x3 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);
    #endregion

    #region Sets Double
    public unsafe void Uniform(int location, in Vector2d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform2(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Quaterniond data, int count = 1)
    {
        fixed (double* ptr = &data.W)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in Vector2d data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector3d data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector4d data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Quaterniond data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Matrix2d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix2x3d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix2x4d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3x2d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix3x4d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4x2d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in Matrix4x3d data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);
    #endregion

    #endregion

    #region SetUniformSN
    public unsafe void Uniform(int location, in System.Numerics.Vector2 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform2(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, this.ValidateUniformLocation(location), count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Matrix3x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Matrix4x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix4(this.BufferID, this.ValidateUniformLocation(location), count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in System.Numerics.Vector2 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in System.Numerics.Vector3 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in System.Numerics.Vector4 data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in System.Numerics.Quaternion data, int count = 1)
        => this.Uniform(this.GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in System.Numerics.Matrix3x2 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);

    public unsafe void Uniform(string name, in System.Numerics.Matrix4x4 data, int count = 1, bool transpose = false)
        => this.Uniform(this.GetUniformLocation(name), data, count, transpose);
    #endregion

    #endregion

    #region ###
    public override string ToString()
    {
        return $"Label: {(string.IsNullOrEmpty(this.Label) ? @"''" : this.Label)} Composion: {this.Composion.Count} Uniforms: {this.ActivesUniforms.Count} Attributes: {this.ActivesAttributes.Count}";
    }

    public void Use()
    {
        GL.UseProgram(this.BufferID);
        IShader.BufferBindedInContext = this.BufferID;
    }

    public void DispatchCompute(uint groupsX = 1, uint groupsY = 1, uint groupsZ = 1)
    {
        this.Use();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
        IShader.ClearContext();
    }

    public void DispatchCompute(int groupsX = 1, int groupsY = 1, int groupsZ = 1)
    {
        this.Use();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
        IShader.ClearContext();
    }

    public void DispatchCompute(DispatchCommand dispatchCommand)
    {
        this.Use();
        GL.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
        IShader.ClearContext();
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
        where TTexture : ITexture
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
        where TTexture : ITexture
    {
        texture.BindToImageUnit(unit, level, layered, layer, TextureAccess);
        this.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Label = " ";
            GL.DeleteProgram(this.BufferID);
            this.BufferID = 0;
            this.ActivesUniforms = new (new Dictionary<string, int>());
            this.ActivesAttributes = new (new Dictionary<string, int>());
            this.Separable = false;
            this.Composion = [];
        }
    }
    #endregion

    #region Create & Proccess
    private static int Create(bool separable, params IShaderSource[] shadersSource)
    {
        int programID = GL.CreateProgram();

        if (separable is true)
        {
            GL.ProgramParameter(programID, ProgramParameterName.ProgramSeparable, 1);
        }

        foreach (var loaders in shadersSource)
        {
            GL.AttachShader(programID, loaders.BufferID);
        }

        GL.LinkProgram(programID);

        if (separable is false)
        {
            foreach (var loaders in shadersSource)
            {
                GL.DetachShader(programID, loaders.BufferID);
                GL.DeleteShader(loaders.BufferID);
            }
        }

        GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            var programInfo = GL.GetProgramInfoLog(programID);
            GL.DeleteProgram(programID);

            throw new ShaderProgramException(programInfo);
        }

        return programID;
    }

    private static ReadOnlyDictionary<string, int> ProcessAttributes(in int programID)
    {
        var attrbsLocations = new Dictionary<string, int> { };
        GL.GetProgram(programID, GetProgramParameterName.ActiveAttributes, out var numberOfAttribs);

        for (var i = 0; i < numberOfAttribs; i++)
        {
            string key = GL.GetActiveAttrib(programID, i, out var _, out var _);
            attrbsLocations.Add(key, GL.GetAttribLocation(programID, key));
        }

        return new ReadOnlyDictionary<string, int>(attrbsLocations);
    }

    private static ReadOnlyDictionary<string, int> ProcessUniforms(in int programID)
    {
        var uniLocations = new Dictionary<string, int> { };
        GL.GetProgram(programID, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        for (var i = 0; i < numberOfUniforms; i++)
        {
            string key = GL.GetActiveUniform(programID, i, out var size, out var _);

            if (size > 1)
            {
                for (int j = 0; j < size; j++)
                {
                    string keyArray = key[..^2] + $"{j}]";

                    int location = GL.GetUniformLocation(programID, keyArray);
                    uniLocations.Add(keyArray, location);
                }
            }
            else
            {
                int location = GL.GetUniformLocation(programID, key);
                uniLocations.Add(key, location);
            }
        }

        return new ReadOnlyDictionary<string, int>(uniLocations);
    }
    #endregion

}
