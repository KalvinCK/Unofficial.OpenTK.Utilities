using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public struct ShaderCompiled : IShaderCompiled, IDisposable
{
    public int BufferID { get; private set; }
    public ShaderType Type { get; private set; }
    public string Name { get; private set; }
    public void Dispose()
    {
        GL.DeleteShader(BufferID);
        BufferID = 0;
        Type = 0;
        Name = "";
    }
    private ShaderCompiled(int shaderID, string shaderName, ShaderType ShaderType)
    {
        this.BufferID = shaderID;
        this.Name = shaderName;
        this.Type = ShaderType;
    }
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
    

    public static ShaderCompiled CompileFromFile(ShaderType ShaderType, string shaderFilePath, string name = "UNAMED")
    {
        //shaderFilePath = Path.Combine("resources/shaders", shaderFilePath);

        string srcShader = IncludeFile(shaderFilePath, name);

        return CompileFromText(ShaderType, srcShader, name);
    }
    public static ShaderCompiled CompileFromText(ShaderType ShaderType, string @srcShader, string name = "UNAMED")
    {
        var shaderID = GL.CreateShader(ShaderType);
        GL.ShaderSource(shaderID, srcShader);
        GL.CompileShader(shaderID);

        GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
        string infoShader = GL.GetShaderInfoLog(shaderID);
        if (code is not (int)All.True)
        {
            GL.DeleteShader(shaderID);

            throw new Exception($"ERROR::COMPILER SHADER: [{name}]!\n" +
                $"TYPE: {ShaderType}.\n" +
                $"INFO: {infoShader}\n");
        }

        return new ShaderCompiled(shaderID, name, ShaderType);
    }
    private static string IncludeFile(string shaderFilePath, in string name)
    {
        if (!File.Exists(shaderFilePath))
            throw new Exception($"ERROR::SHADER COMPILE::[{name}] FILE NOT FOUND:: [{shaderFilePath}]\n");


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
                string PathIncludeShader = match.Groups[1].Value;

                string srcIncludeShader = File.ReadAllText(Path.Combine(absolutePath, PathIncludeShader));

                lines.RemoveAt(indexLine);
                lines.Insert(indexLine, srcIncludeShader);
            }

        }
        
        return string.Join(Environment.NewLine, lines);
    }
}
