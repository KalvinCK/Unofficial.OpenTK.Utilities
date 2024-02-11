using System.Collections.ObjectModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities;

public class Shader : IShader, IDisposable
{
    public Shader(string name, params ShaderCompiled[] shadersCompileds)
    {
        this.Name = string.IsNullOrEmpty(name) ? IBuffer.Unnamed : name;
        this.BufferID = Create(this.Name, true, shadersCompileds);
        this.UniformsLocations = ProcessUniforms(this.BufferID);
    }

    public Shader(params ShaderCompiled[] shadersCompileds)
    {
        this.BufferID = Create(this.Name, true, shadersCompileds);
        this.UniformsLocations = ProcessUniforms(this.BufferID);
    }

    private Shader(string name, int programID, ReadOnlyDictionary<string, int> uniformLocations)
    {
        this.Name = name;
        this.BufferID = programID;
        this.UniformsLocations = uniformLocations;
    }

    public int BufferID { get; private set; }

    public ReadOnlyDictionary<string, int> UniformsLocations { get; private set; }

    public int UniformsCount => this.UniformsLocations.Count;

    public string Name { get; set; } = IBuffer.Unnamed;

    public bool ContainsUniform(string name)
    {
        return this.UniformsLocations.ContainsKey(name);
    }

    public int GetAttribute(string name)
    {
        int attrib = GL.GetAttribLocation(this.BufferID, name);

        if (attrib == -1)
        {
            Helpers.PrintWarning($"Attribute [{name}] not found in the shader: [{this.Name}]");
        }

        return attrib;
    }

    public int GetUniformLocation(string name)
    {
        bool result = this.UniformsLocations.TryGetValue(name, out int value);

        if (result)
        {
            Helpers.PrintWarning($"Uniform: [{name}] does not exist in the shader: [{this.Name}]");
        }

        return result ? value : -1;
    }

    #region Uploads
    public void Uniform(int location, int data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, location, count, ref data);

    public void Uniform(int location, uint data, int count = 1)
        => GL.ProgramUniform1((uint)this.BufferID, location, count, ref data);

    public void Uniform(int location, float data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, location, count, ref data);

    public void Uniform(int location, double data, int count = 1)
        => GL.ProgramUniform1(this.BufferID, location, count, ref data);

    public void Uniform(int location, long data, int count = 1)
        => GL.Arb.ProgramUniformHandle(this.BufferID, location, count, ref data);

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
            GL.ProgramUniform2(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
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
            GL.ProgramUniform2(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.W)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x3 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(this.BufferID, location, count, transpose, ptr);
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
            GL.ProgramUniform2(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector3d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Vector4d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Quaterniond data, int count = 1)
    {
        fixed (double* ptr = &data.W)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix2x4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix3x4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x2d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in Matrix4x3d data, int count = 1, bool transpose = false)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(this.BufferID, location, count, transpose, ptr);
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
            GL.ProgramUniform2(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(this.BufferID, location, count, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Matrix3x2 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix3x2(this.BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(int location, in System.Numerics.Matrix4x4 data, int count = 1, bool transpose = false)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix4(this.BufferID, location, count, transpose, ptr);
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

    #region Ctx
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
            GL.DeleteProgram(this.BufferID);
            this.BufferID = 0;
            this.Name = IBuffer.Unnamed;
            this.UniformsLocations = new (new Dictionary<string, int>());
        }
    }
    #endregion

    #region Create
    private static int Create(in string name, bool DeleteCompildedShader, params ShaderCompiled[] shadersCompileds)
    {
        int programID = GL.CreateProgram();

        foreach (var shaderCompiled in shadersCompileds)
        {
            GL.AttachShader(programID, shaderCompiled.BufferID);
        }

        GL.LinkProgram(programID);

        foreach (var shaderCompiled in shadersCompileds)
        {
            Detach(programID, DeleteCompildedShader, shaderCompiled);
        }

        GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            foreach (var shaderCompiled in shadersCompileds)
            {
                Detach(programID, DeleteCompildedShader, shaderCompiled);
            }

            var programInfo = GL.GetProgramInfoLog(programID);
            GL.DeleteProgram(programID);

            throw new Exception($"Failed to create shader program." +
                $"name: [{name}].\n" +
                $"info: {programInfo}.");
        }

        return programID;
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

    private static void Detach(in int programID, in bool DeleteCompildedShader, in ShaderCompiled shaderCompiled)
    {
        if (DeleteCompildedShader)
        {
            GL.DetachShader(programID, shaderCompiled.BufferID);
            shaderCompiled.Dispose();
        }
    }
    #endregion

}
