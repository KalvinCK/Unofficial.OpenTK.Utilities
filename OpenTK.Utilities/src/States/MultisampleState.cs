namespace OpenTK.Utilities;

public struct MultisampleState()
{
    public bool Enable = false;                 // glEnable()
    public bool SampleShadingEnable = false;    // glEnable(GL_SAMPLE_SHADING)
    public bool AlphaToCoverageEnable = false;  // glEnable(GL_SAMPLE_ALPHA_TO_COVERAGE)
    public bool AlphaToOneEnable = false;       // glEnable(GL_SAMPLE_ALPHA_TO_ONE)
    public float MinSampleShading = 1;          // glMinSampleShading
    public int SampleMaskNumber = 0;            // glSampleMask
    public int SampleMaskValue = 1;             // glSampleMask
}
