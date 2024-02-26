namespace OpenTK.Utilities;

public interface IReadOnlyBuffer
{
    public int BufferID { get; }

    public virtual bool Equals(IReadOnlyBuffer other)
    {
        return this.BufferID == other.BufferID;
    }

    public virtual bool NotEquals(IReadOnlyBuffer other)
    {
        return this.BufferID != other.BufferID;
    }
}
