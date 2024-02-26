using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public struct RasterizationState()
{
    public static readonly RasterizationState Default = new ();

    public bool DepthClampEnable = false;
    public bool DepthBiasEnable = false;

    public bool CullFaceEnable = false;
    public CullFaceMode CullMode = CullFaceMode.Back;

    public PolygonMode PolygonMode = PolygonMode.Fill;
    public FrontFaceDirection FrontFaceDir = FrontFaceDirection.Ccw;
    public float DepthBiasConstantFactor = 0;
    public float DepthBiasSlopeFactor = 0;

    public bool LineSmooth = false;
    public float PointSize = 1;
    public float LineWidth = 1;
}
