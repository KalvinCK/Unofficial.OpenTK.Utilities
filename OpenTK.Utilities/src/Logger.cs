namespace OpenTK.Utilities;

public struct Logger : ILogger
{
    private const string DateFormat = "HH:mm:ss.f";

    private readonly StreamWriter streamFile;

    private bool disposed;

    public Logger(string? path, string fileName)
    {
        this.FilePath = Path.Combine(path ?? Directory.GetCurrentDirectory(), fileName) + ".logger.txt";
        this.streamFile = new StreamWriter(File.Create(this.FilePath));
    }

    public enum Flag : int
    {
        Instruction,
        Information,
        Warning,
        Error,
        Fatal,
    }

    public string FilePath { get; }

    public readonly void Write(Flag flag, string text)
    {
        this.Write<Flag>(flag, text);
    }

    public readonly void Write<TEnum>(TEnum flag, string text)
        where TEnum : Enum
    {
        text = string.Format("{0} {1} INFO: {2}", DateTime.Now.ToString(DateFormat), flag.ToString().ToUpper(), text);

        Console.WriteLine(text);
        this.streamFile.WriteLine(text);
        this.streamFile.Flush();
    }

    public void Dispatch()
    {
        if (!this.disposed)
        {
            this.disposed = true;
            this.streamFile.Dispose();
        }
    }
}
