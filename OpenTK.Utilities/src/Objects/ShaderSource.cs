using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class ShaderSource : IShaderSource, IDisposable
{
    private ShaderSource(int shaderID, ShaderType shaderType)
    {
        this.BufferID = shaderID;
        this.Type = shaderType;
    }

    public int BufferID { get; private set; }

    public ShaderType Type { get; private set; }

    #region File
    public static ShaderSource FromFileVertex(string shaderFilePath)
        => FromFile(ShaderType.VertexShader, shaderFilePath);

    public static ShaderSource FromFileFragment(string shaderFilePath)
        => FromFile(ShaderType.FragmentShader, shaderFilePath);

    public static ShaderSource FromFileGeometry(string shaderFilePath)
        => FromFile(ShaderType.GeometryShader, shaderFilePath);

    public static ShaderSource FromFileTessEvaluation(string shaderFilePath)
        => FromFile(ShaderType.TessEvaluationShader, shaderFilePath);

    public static ShaderSource FromFileTessControl(string shaderFilePath)
        => FromFile(ShaderType.TessControlShader, shaderFilePath);

    public static ShaderSource FromFileCompute(string shaderFilePath)
        => FromFile(ShaderType.ComputeShader, shaderFilePath);

    public static ShaderSource FromFile(ShaderType shaderType, string shaderFilePath)
    {
        string srcShader = ProcessText(shaderFilePath);
        return FromText(shaderType, srcShader);
    }
    #endregion

    #region Text
    public static ShaderSource FromTextVertex(string @srcShader)
        => FromText(ShaderType.VertexShader, srcShader);

    public static ShaderSource FromTextFragment(string @srcShader)
        => FromText(ShaderType.FragmentShader, srcShader);

    public static ShaderSource FromTextGeometry(string @srcShader)
        => FromText(ShaderType.GeometryShader, srcShader);

    public static ShaderSource FromTextTessEvaluation(string @srcShader)
        => FromText(ShaderType.TessEvaluationShader, srcShader);

    public static ShaderSource FromTextTessControl(string @srcShader)
        => FromText(ShaderType.TessControlShader, srcShader);

    public static ShaderSource FromTextCompute(string @srcShader)
        => FromText(ShaderType.ComputeShader, srcShader);

    public static ShaderSource FromText(ShaderType shaderType, string @srcShader)
    {
        var shaderID = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderID, srcShader);
        GL.CompileShader(shaderID);

        GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
        if (code is not (int)All.True)
        {
            string infoShader = GL.GetShaderInfoLog(shaderID);
            GL.DeleteShader(shaderID);

            throw new ShaderCompilerException(shaderType, infoShader);
        }

        return new ShaderSource(shaderID, shaderType);
    }
    #endregion

    #region Binary
    public static ShaderSource FromBinaryVertex(byte[] binarySrc)
        => FromBinary(ShaderType.VertexShader, binarySrc);

    public static ShaderSource FromBinaryFragment(byte[] binarySrc)
        => FromBinary(ShaderType.FragmentShader, binarySrc);

    public static ShaderSource FromBinaryGeometry(byte[] binarySrc)
        => FromBinary(ShaderType.GeometryShader, binarySrc);

    public static ShaderSource FromBinaryTessEvaluation(byte[] binarySrc)
        => FromBinary(ShaderType.TessEvaluationShader, binarySrc);

    public static ShaderSource FromBinaryTessControl(byte[] binarySrc)
        => FromBinary(ShaderType.TessControlShader, binarySrc);

    public static ShaderSource FromBinaryCompute(byte[] binarySrc)
        => FromBinary(ShaderType.ComputeShader, binarySrc);

    public static ShaderSource FromBinary(ShaderType shaderType, byte[] binarySrc)
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

        return new ShaderSource(shaderID, shaderType);
    }
    #endregion

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
