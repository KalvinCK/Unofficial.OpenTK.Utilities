namespace OpenTK.Utilities;

public interface IBuffer
{
    internal static string Unnamed = "UNNAMED";

    public int BufferID { get; }
}

public readonly struct BufferRef(int bufferID) : IBuffer
{
    public int BufferID { get; } = bufferID;
}
