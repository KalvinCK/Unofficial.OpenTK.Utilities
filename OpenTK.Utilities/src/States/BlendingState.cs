using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

/// <summary>
/// Used in <see cref="GL.ColorMask(bool, bool, bool, bool)"/>.
/// </summary>
public struct MaskComponent(bool red, bool green, bool blue, bool alpha)
{
    public static readonly MaskComponent EnableAll = new (true, true, true, true);
    public static readonly MaskComponent DisableAll = new (false, false, false, false);
    public static readonly MaskComponent EnableRed = new (true, false, false, false);
    public static readonly MaskComponent EnableGreen = new (false, true, false, false);
    public static readonly MaskComponent EnableBlue = new (false, false, true, false);
    public static readonly MaskComponent EnableAlpha = new (false, false, false, true);
    public static readonly MaskComponent DisableAlpha = new (true, true, true, false);

    public bool Red = red;
    public bool Green = green;
    public bool Blue = blue;
    public bool Alpha = alpha;
}

/// <summary>
/// Defines blending parameters.
/// </summary>
/// <remarks>
/// Use in  GLBlendFuncSeparatei + glBlendEquationSeparatei.
/// </remarks>
public struct BlendingState()
{
    public bool Enable = false; // if false, blend factor = one?
    public uint BufIndex = 0; // the index of the draw buffer

    public MaskComponent MaskComps = MaskComponent.EnableAll; // glColorMaski

    public BlendingFactorSrc SrcColorBlendFactor = BlendingFactorSrc.One; // srcRGB
    public BlendingFactorSrc SrcAlphaBlendFactor = BlendingFactorSrc.One; // srcAlpha

    public BlendingFactorDest DstColorBlendFactor = BlendingFactorDest.Zero; // dstRGB
    public BlendingFactorDest DstAlphaBlendFactor = BlendingFactorDest.Zero; // dstAlpha

    public BlendEquationMode ColorBlendMode = BlendEquationMode.FuncAdd; // modeRGB
    public BlendEquationMode AlphaBlendMode = BlendEquationMode.FuncAdd; // modeAlpha
}
