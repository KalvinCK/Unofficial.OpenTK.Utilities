using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public struct DepthState()
{
    /// <summary>
    /// { Enable, Disable } GL.Enable(GL_DEPTH_TEST).
    /// </summary>
    public bool DepthTestEnable = false;           // gl{Enable, Disable}(GL_DEPTH_TEST)
    public bool DepthWriteEnable = false;           // glDepthMask(depthWriteEnable)
    public DepthFunction DepthCompareOp = DepthFunction.Less; // glDepthFunc
}
