using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public struct StencilFaceState()
{
    /// <summary>
    /// Specifies the action to take when the stencil test fails.
    /// </summary>
    /// <remarks>
    /// Use in <see cref="GL.StencilOpSeparate(StencilFace, StencilOp, StencilOp, StencilOp)"/> (sfail).
    /// </remarks>
    public StencilOp PassOp = StencilOp.Keep;

    /// <summary>
    /// Specifies the stencil action when the stencil test passes, but the depth test fails.
    /// dpfail accepts the same symbolic constants as sfail. The initial value is . GL_KEEP.
    /// </summary>
    /// <remarks>
    /// Use in <see cref="GL.StencilOpSeparate(StencilFace, StencilOp, StencilOp, StencilOp)"/> (dpfail).
    /// </remarks>
    public StencilOp FailOp = StencilOp.Keep;

    /// <summary>
    /// Specifies the stencil action when both the stencil test and the depth test pass,
    /// or when the stencil test passes and either there is no depth buffer or depth testing
    /// is not enabled. dppass accepts the same symbolic constants as sfail.
    /// </summary>
    /// <remarks>
    /// Use in <see cref="GL.StencilOpSeparate(StencilFace, StencilOp, StencilOp, StencilOp)"/> (dppss).
    /// </remarks>
    public StencilOp DepthFailOp = StencilOp.Keep;

    /// <summary>
    /// Specifies the test function.
    /// </summary>
    /// <remarks>
    /// <see cref="GL.StencilFuncSeparate(StencilFace, StencilFunction, int, int)"/> (func).
    /// </remarks>
    public StencilFunction CompareOp = StencilFunction.Always;

    /// <summary>
    /// Specifies the reference value for the stencil test. Stencil comparison
    /// operations and queries of ref clamp its value to the range [0,2n−1],
    /// where n is the number of bitplanes in the stencil buffer.
    /// </summary>
    /// <remarks>
    /// <see cref="GL.StencilFuncSeparate(StencilFace, StencilFunction, int, int)"/> (ref).
    /// </remarks>
    public int Reference = 0;

    /// <summary>
    /// Specifies a mask that is ANDed with both the reference value and the stored stencil value when the test is done.
    /// </summary>
    /// <remarks>
    /// <see cref="GL.StencilFuncSeparate(StencilFace, StencilFunction, int, int)"/> (mask).
    /// </remarks>
    public int CompareMask = 0;

    /// <summary>
    /// Specifies a mask that is ANDed with both the reference value and the stored stencil value when the test is done.
    /// </summary>
    /// <remarks>
    /// <see cref="GL.StencilMaskSeparate(StencilFace, int)"/>.
    /// </remarks>
    public int WriteMask = 1;
}

/// <summary>
/// Blending Settings <see cref="StencilFace.FrontAndBack"/>.
/// </summary>
public struct StencilState()
{
    public bool Enable = false;

    /// <summary>
    /// Blending Settings only <see cref="StencilFace.Front"/>.
    /// </summary>
    public StencilFaceState Front;

    /// <summary>
    /// Blending Settings only <see cref="StencilFace.Back"/>.
    /// </summary>
    public StencilFaceState Back;
}
