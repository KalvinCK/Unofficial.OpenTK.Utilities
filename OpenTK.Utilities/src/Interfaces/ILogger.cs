namespace OpenTK.Utilities;

public interface ILogger
{
    public string FilePath { get; }

    public void Write<TEnum>(TEnum flag, string text)
        where TEnum : Enum;

    public void Dispatch();
}
