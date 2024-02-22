using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Objects;

public interface IQuery : IBuffer
{
    QueryTarget Target { get; }

    internal static int Create(QueryTarget QueryTarget)
    {
        GL.CreateQueries(QueryTarget, 1, out int buffer);
        return buffer;
    }
}
