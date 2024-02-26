using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public class QueryObject : IDisposable
{
    public QueryObject(QueryTarget QueryTarget)
    {
        this.Target = QueryTarget;
        GL.CreateQueries(QueryTarget, 1, out int buffer);
        this.BufferID = buffer;
    }

    public QueryTarget Target { get; private set; }

    public int BufferID { get; private set; }

    public void Begin()
    {
        GL.BeginQuery(this.Target, this.BufferID);
    }

    public void End()
    {
        GL.EndQuery(this.Target);
    }

    public void GetObject(GetQueryObjectParam GetQueryObjectParam, out int result)
    {
        GL.GetQueryObject(this.BufferID, GetQueryObjectParam, out result);
    }

    public void GetObject(GetQueryObjectParam GetQueryObjectParam, out long result)
    {
        GL.GetQueryObject(this.BufferID, GetQueryObjectParam, out result);
    }

    public void GetObject(GetQueryObjectParam GetQueryObjectParam, ref int[] result)
    {
        GL.GetQueryObject(this.BufferID, GetQueryObjectParam, result);
    }

    public void GetObject(GetQueryObjectParam GetQueryObjectParam, ref long[] result)
    {
        GL.GetQueryObject(this.BufferID, GetQueryObjectParam, result);
    }

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
