using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class Query : IQuery, IDisposable
{
    public Query(QueryTarget QueryTarget)
    {
        this.Target = QueryTarget;
        this.BufferID = IQuery.Create(QueryTarget);
    }

    public QueryTarget Target { get; private set; }

    public int BufferID { get; private set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteQuery(this.BufferID);
            this.BufferID = 0;
            this.Target = 0;
        }
    }
}
