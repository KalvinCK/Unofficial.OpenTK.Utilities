using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

#pragma warning disable SA1310 // Field names should not contain underscore
public struct PixelDescription
{
    #region Template Sample
    public static readonly PixelDescription RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba8,
        Format = PixelFormat.Rgba,
        Type = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb8,
        Format = PixelFormat.Rgb,
        Type = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg8,
        Format = PixelFormat.Rg,
        Type = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R8,
        Format = PixelFormat.Red,
        Type = PixelType.UnsignedByte,
    };
    #endregion

    #region Template Sample Gamma
    public static readonly PixelDescription SRGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Srgb8Alpha8,
        Format = PixelFormat.Rgba,
        Type = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription SRGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Srgb8,
        Format = PixelFormat.Rgb,
        Type = PixelType.UnsignedByte,
    };
    #endregion

    #region Template HDR
    public static readonly PixelDescription HDR_RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba32f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription HDR_RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb32f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription HDR_RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg32f,
        Format = PixelFormat.Rg,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription HDR_R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R32f,
        Format = PixelFormat.Red,
        Type = PixelType.Float,
    };
    #endregion

    #region Template HALF HDR
    public static readonly PixelDescription Half_HDR_RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba16f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription Half_HDR_RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb16f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription Half_HDR_RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg16f,
        Format = PixelFormat.Rg,
        Type = PixelType.Float,
    };

    public static readonly PixelDescription Half_HDR_R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R16f,
        Format = PixelFormat.Red,
        Type = PixelType.Float,
    };
    #endregion

    public SizedInternalFormat InternalFormat { get; set; }

    public PixelFormat Format { get; set; }

    public PixelType Type { get; set; }
}
#pragma warning restore SA1310 // Field names should not contain underscore
