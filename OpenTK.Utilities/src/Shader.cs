using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenTK.Utilities.Textures;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace OpenTK.Utilities;

public class Shader : IShader, IDisposable
{
    public int BufferID { get; private set; }
    public ReadOnlyDictionary<string, int> UniformsLocations { get; }
    public int UniformsCount => UniformsLocations.Count;
    public string Name { get; set; } = "UNNAMED";
    public Shader(string name, params ShaderCompiled[] shadersCompileds)
    {
        Name = string.IsNullOrEmpty(name) ? "UNNAMED" : name;
        BufferID = Create(Name, true, shadersCompileds);
        UniformsLocations = ProcessUniforms(BufferID);
    }
    public Shader(params ShaderCompiled[] shadersCompileds)
    {
        BufferID = Create(Name, true, shadersCompileds);
        UniformsLocations = ProcessUniforms(BufferID);
    }

    private Shader(string name, int programID, ReadOnlyDictionary<string, int> uniformLocations)
    {
        Name = name;
        BufferID = programID;
        UniformsLocations = uniformLocations;
    }
    public static Shader CreateProgram(string name, params ShaderCompiled[] shadersCompileds)
    {
        var NewName = string.IsNullOrEmpty(name) ? "UNNAMED" : name;
        var programID = Create(NewName, false, shadersCompileds);
        return new Shader(NewName, programID, ProcessUniforms(programID));
    }

    #region Create
    private static int Create(in string name, bool DeleteCompildedShader, params ShaderCompiled[] shadersCompileds)
    {
        int programID = GL.CreateProgram();

        foreach (var shaderCompiled in shadersCompileds)
            GL.AttachShader(programID, shaderCompiled.BufferID);

        GL.LinkProgram(programID);


        foreach(var shaderCompiled in shadersCompileds)
            Detach(programID, DeleteCompildedShader, shaderCompiled);


        GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out var code);
        var programInfo = GL.GetProgramInfoLog(programID);
        if (code != (int)All.True)
        {
            foreach (var shaderCompiled in shadersCompileds)
            {
                Detach(programID, DeleteCompildedShader, shaderCompiled);
            }

            GL.DeleteProgram(programID);

            throw new Exception($"ERROR::CREATE SHADER PROGRAM: {name}.\nINFO: {programInfo}");
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
                    //string keyArray = string.Concat(key.AsSpan(0, key.Length - 2), $"{j}]");
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
        if(DeleteCompildedShader)
        {
            GL.DetachShader(programID, shaderCompiled.BufferID);
            shaderCompiled.Dispose();
        }
    }
    #endregion

    #region Ctx
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if(disposing)
        {
            GL.DeleteProgram(BufferID);
            BufferID = 0;
        }
    }
    public void Use()
    {
        GL.UseProgram(BufferID);
        IShader.BufferBindedInContext = BufferID;
    }
    
    public void DispatchCompute(uint groupsX = 1, uint groupsY = 1, uint groupsZ = 1)
    {
        Use();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
        IShader.ClearContext();
    }
    public void DispatchCompute(int groupsX = 1, int groupsY = 1, int groupsZ = 1)
    {
        Use();
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
        IShader.ClearContext();
    }
    public void DispatchCompute(DispatchCommand dispatchCommand)
    {
        Use();
        GL.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
        IShader.ClearContext();
    }

    public void DispatchCompute<TTexture>(
        TTexture Texture,
        int groupsX = 1, int groupsY = 1, int groupsZ = 1,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly
        ) where TTexture : ITexture
    {
        Texture.BindToImageUnit(unit, level, layered, layer, TextureAccess);
        this.DispatchCompute(groupsX, groupsY, groupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);
    }
    public void DispatchCompute<TTexture>(
        TTexture Texture,
        DispatchCommand dispatchCommand,
        int unit = 0,
        bool layered = false,
        int layer = 0,
        int level = 0,
        TextureAccess TextureAccess = TextureAccess.WriteOnly
        ) where TTexture : ITexture
    {
        Texture.BindToImageUnit(unit, level, layered, layer, TextureAccess);
        this.DispatchCompute(dispatchCommand.NumGroupsX, dispatchCommand.NumGroupsY, dispatchCommand.NumGroupsZ);
        GL.MemoryBarrier(MemoryBarrierFlags.TextureFetchBarrierBit);
    }

    #endregion

    #region Uploads
    public bool ContainsUniform(string name)
    {
        return UniformsLocations.ContainsKey(name);
    }
    public int GetAttribute(string name)
    {
        int attrib = GL.GetAttribLocation(BufferID, name);

        if (attrib == -1)
            WriteError($"Attribute [{name}] not found in the shader: [{Name}]");

        return attrib;
    }
    public int GetUniformLocation(string name)
    {
        bool result = UniformsLocations.TryGetValue(name, out int value);

        value = result ? value : -1;

        if (value == -1)
            WriteError($"Uniform: [{name}] does not exist in the shader: [{Name}]");

        return value;
    }

    [Conditional("DEBUG")]
    private static void WriteError(string msg)
    {
        Debug.Print(msg);
    }

    public void Uniform(int location, int data, int count = 1)
        => GL.ProgramUniform1(BufferID, location, count, ref data);
    public void Uniform(int location, uint data, int count = 1)
        => GL.ProgramUniform1((uint)BufferID, location, count, ref data);
    public void Uniform(int location, float data, int count = 1)
        => GL.ProgramUniform1(BufferID, location, count, ref data);
    public void Uniform(int location, double data, int count = 1)
        => GL.ProgramUniform1(BufferID, location, count, ref data);
    public void Uniform(int location, long data, int count = 1)
        => GL.Arb.ProgramUniformHandle(BufferID, location, count, ref data);
    public void Uniform(int location, bool data)
        => this.Uniform(location, data ? 1 : 0);
    public void Uniform<TEnum>(int location, TEnum enumData) where TEnum : Enum
        => this.Uniform(location, (int)(object)enumData);
    public unsafe void Uniform(int location, in Size data)
        => this.Uniform(location, new Vector2i(data.Width, data.Height));
    public unsafe void Uniform(int location, in Point data)
        => this.Uniform(location, new Vector2i(data.X, data.Y));
    public unsafe void Uniform(int location, in SizeF data)
        => this.Uniform(location, new Vector2(data.Width, data.Height));
    public unsafe void Uniform(int location, in PointF data)
        => this.Uniform(location, new Vector2(data.X, data.Y));

    public void Uniform(string name, int data, int count = 1)
        => this.Uniform(GetUniformLocation(name), data, count);
    public void Uniform(string name, uint data, int count = 1)
        => this.Uniform(GetUniformLocation(name), data, count);
    public void Uniform(string name, float data, int count = 1)
        => this.Uniform(GetUniformLocation(name), data, count);
    public void Uniform(string name, double data, int count = 1)
        => this.Uniform(GetUniformLocation(name), data, count);
    public void Uniform(string name, long data, int count = 1)
        => this.Uniform(GetUniformLocation(name), data, count);
    public void Uniform(string name, bool data)
        => this.Uniform(name, data ? 1 : 0, 1);
    public void Uniform<TEnum>(string name, TEnum enumData) where TEnum : Enum
        => this.Uniform(name, (int)(object)enumData);
    public unsafe void Uniform(string name, in Size data)
        => this.Uniform(GetUniformLocation(name), data);
    public unsafe void Uniform(string name, in SizeF data)
        => this.Uniform(GetUniformLocation(name), data);
    public unsafe void Uniform(string name, in Point data)
        => this.Uniform(GetUniformLocation(name), data);
    public unsafe void Uniform(string name, in PointF data)
        => this.Uniform(GetUniformLocation(name), new Vector2(data.X, data.Y));

    #endregion

    #region SetUniformTK

    #region Sets Float
    public unsafe void Uniform(int location, in Vector2 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform2(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.W)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2x3 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2x4 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3x2 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3x4 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4x2 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4x3 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in Vector2 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Vector3 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Vector4 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Quaternion data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Matrix2 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix2x3 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix2x4 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3x2 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3x4 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4x2 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4x3 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    #endregion

    #region Sets Int
    public unsafe void Uniform(int location, in Vector2i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform2(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector3i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform3(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector4i data, int count = 1)
    {
        fixed (int* ptr = &data.X)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    #endregion

    #region Sets Double
    public unsafe void Uniform(int location, in Vector2d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform2(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector3d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform3(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Vector4d data, int count = 1)
    {
        fixed (double* ptr = &data.X)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Quaterniond data, int count = 1)
    {
        fixed (double* ptr = &data.W)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2x3d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x3(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix2x4d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix2x4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3x2d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix3x4d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix3x4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4x2d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x2(BufferID, location, count, transpose, ptr);
        }
    }
    public unsafe void Uniform(int location, in Matrix4x3d data, int count = 1, bool transpose = true)
    {
        fixed (double* ptr = &data.Row0.X)
        {
            GL.ProgramUniformMatrix4x3(BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in Vector2i data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Vector2d data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector3i data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Vector3d data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Vector4i data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in Vector4d data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Quaterniond data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);

    public unsafe void Uniform(string name, in Matrix2d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix2x3d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix2x4d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3x2d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix3x4d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4x2d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in Matrix4x3d data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    #endregion

    #endregion

    #region SetUniformSN
    public unsafe void Uniform(int location, in System.Numerics.Vector2 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform2(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in System.Numerics.Vector3 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform3(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in System.Numerics.Vector4 data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    
    public unsafe void Uniform(int location, in System.Numerics.Quaternion data, int count = 1)
    {
        fixed (float* ptr = &data.X)
        {
            GL.ProgramUniform4(BufferID, location, count, ptr);
        }
    }
    public unsafe void Uniform(int location, in System.Numerics.Matrix3x2 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix3x2(BufferID, location, count, transpose, ptr);
        }
    }
    
    public unsafe void Uniform(int location, in System.Numerics.Matrix4x4 data, int count = 1, bool transpose = true)
    {
        fixed (float* ptr = &data.M11)
        {
            GL.ProgramUniformMatrix4(BufferID, location, count, transpose, ptr);
        }
    }

    public unsafe void Uniform(string name, in System.Numerics.Vector2 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in System.Numerics.Vector3 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in System.Numerics.Vector4 data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in System.Numerics.Quaternion data, int count = 1)
        => Uniform(GetUniformLocation(name), data, count);
    public unsafe void Uniform(string name, in System.Numerics.Matrix3x2 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    public unsafe void Uniform(string name, in System.Numerics.Matrix4x4 data, int count = 1, bool transpose = true)
        => Uniform(GetUniformLocation(name), data, count, transpose);
    #endregion

}