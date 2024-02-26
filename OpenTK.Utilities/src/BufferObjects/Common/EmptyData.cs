namespace OpenTK.Utilities.BufferObjects;

internal struct EmptyData<T> : IDisposable
    where T : struct
{
    public T[] Data;

    public EmptyData(int count)
    {
        this.Data = new T[count];
    }

    public EmptyData(T[] arrs)
    {
        this.Data = arrs;
    }

    public readonly int Count => this.Data.Length;

    public void Dispose() => this.Data = [];
}
