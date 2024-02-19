﻿using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class ShaderCompiled : IShaderCompiled, IDisposable
{
    private ShaderCompiled(int shaderID, string shaderName, ShaderType shaderType)
    {
        this.BufferID = shaderID;
        this.Name = shaderName;
        this.Type = shaderType;
    }

    public int BufferID { get; private set; }

    public ShaderType Type { get; private set; }

    public string Name { get; private set; }

    #region File
    public static ShaderCompiled CompileFromFileVertex(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.VertexShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFileFragment(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.FragmentShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFileGeometry(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.GeometryShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFileTessEvaluation(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.TessEvaluationShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFileTessControl(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.TessControlShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFileCompute(string shaderFilePath, string name = "UNAMED")
        => CompileFromFile(ShaderType.ComputeShader, shaderFilePath, name);

    public static ShaderCompiled CompileFromFile(ShaderType shaderType, string shaderFilePath, string name = "UNAMED")
    {
        string srcShader = ProcessText(shaderFilePath);
        return CompileFromText(shaderType, srcShader, name);
    }
    #endregion

    #region Text
    public static ShaderCompiled CompileFromTextVertex(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.VertexShader, srcShader, name);

    public static ShaderCompiled CompileFromTextFragment(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.FragmentShader, srcShader, name);

    public static ShaderCompiled CompileFromTextGeometry(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.GeometryShader, srcShader, name);

    public static ShaderCompiled CompileFromTextTessEvaluation(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.TessEvaluationShader, srcShader, name);

    public static ShaderCompiled CompileFromTextTessControl(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.TessControlShader, srcShader, name);

    public static ShaderCompiled CompileFromTextCompute(string @srcShader, string name = "UNAMED")
        => CompileFromText(ShaderType.ComputeShader, srcShader, name);

    public static ShaderCompiled CompileFromText(ShaderType shaderType, string @srcShader, string name = "UNAMED")
    {
        var shaderID = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderID, srcShader);
        GL.CompileShader(shaderID);

        GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
        if (code is not (int)All.True)
        {
            string infoShader = GL.GetShaderInfoLog(shaderID);
            GL.DeleteShader(shaderID);

            throw new Exception($"Failed to compile shader: " +
                $"name: {name}.\n" +
                $"type: {shaderType}.\n" +
                $"GL info: {infoShader}.");
        }

        return new ShaderCompiled(shaderID, name, shaderType);
    }
    #endregion

    #region Binary
    public static ShaderCompiled CompileFromBinaryVertex(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.VertexShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinaryFragment(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.FragmentShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinaryGeometry(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.GeometryShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinaryTessEvaluation(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.TessEvaluationShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinaryTessControl(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.TessControlShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinaryCompute(byte[] binarySrc, string name = "UNAMED")
        => CompileFromBinary(ShaderType.ComputeShader, binarySrc, name);

    public static ShaderCompiled CompileFromBinary(ShaderType shaderType, byte[] binarySrc, string name = "UNAMED")
    {
        var shaderID = GL.CreateShader(shaderType);
        GL.ShaderBinary(1, ref shaderID, BinaryFormat.ShaderBinaryFormatSpirV, binarySrc, binarySrc.Length);
        GL.CompileShader(shaderID);

        GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
        if (code is not (int)All.True)
        {
            string infoShader = GL.GetShaderInfoLog(shaderID);
            GL.DeleteShader(shaderID);

            throw new ShaderCompilerException(shaderType, infoShader);
        }

        return new ShaderCompiled(shaderID, name, shaderType);
    }
    #endregion

    public override string ToString()
    {
        return $"Name: {this.Name} Type: {this.Type}";
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteShader(this.BufferID);
            this.BufferID = 0;
            this.Type = 0;
            this.Name = " ";
        }
    }

    private static string ProcessText(string shaderFilePath)
    {
        if (!File.Exists(shaderFilePath))
        {
            throw new FileNotFoundException(shaderFilePath);
        }

        var lines = new List<string>(File.ReadAllLines(shaderFilePath));

        const string include = "#include";

        var absolutePath = Path.GetDirectoryName(shaderFilePath)!;

        int indexLine = lines.FindIndex(linha => linha.Contains(include));

        if (indexLine is not -1)
        {
            var subs = lines[indexLine][include.Length..];

            Regex regex = new Regex("\"([^\"]*)\"");
            Match match = regex.Match(subs);

            if (match.Success)
            {
                string pathIncludeShader = match.Groups[1].Value;

                string srcIncludeShader = File.ReadAllText(Path.Combine(absolutePath, pathIncludeShader));

                lines.RemoveAt(indexLine);
                lines.Insert(indexLine, srcIncludeShader);
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}