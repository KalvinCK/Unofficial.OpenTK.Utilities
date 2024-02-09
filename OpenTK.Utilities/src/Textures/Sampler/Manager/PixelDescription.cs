using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct PixelDescription
{
    public required SizedInternalFormat InternalFormat;
    public required PixelFormat Format;
    public required PixelType Type;

    #region Template Sample
    public readonly static PixelDescription RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba8,
        Format = PixelFormat.Rgba,
        Type = PixelType.UnsignedByte
    };

    public readonly static PixelDescription RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb8,
        Format = PixelFormat.Rgb,
        Type = PixelType.UnsignedByte
    };

    public readonly static PixelDescription RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg8,
        Format = PixelFormat.Rg,
        Type = PixelType.UnsignedByte
    };

    public readonly static PixelDescription R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R8,
        Format = PixelFormat.Red,
        Type = PixelType.UnsignedByte
    };
    #endregion

    #region Template Sample Gamma
    public readonly static PixelDescription SRGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Srgb8Alpha8,
        Format = PixelFormat.Rgba,
        Type = PixelType.UnsignedByte
    };

    public readonly static PixelDescription SRGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Srgb8,
        Format = PixelFormat.Rgb,
        Type = PixelType.UnsignedByte
    };
    #endregion

    #region Template HDR
    public readonly static PixelDescription HDR_RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba32f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float
    };

    public readonly static PixelDescription HDR_RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb32f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float
    };

    public readonly static PixelDescription HDR_RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg32f,
        Format = PixelFormat.Rg,
        Type = PixelType.Float
    };

    public readonly static PixelDescription HDR_R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R32f,
        Format = PixelFormat.Red,
        Type = PixelType.Float
    };
    #endregion

    #region Template HALF HDR
    public readonly static PixelDescription Half_HDR_RGBA = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgba16f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float
    };

    public readonly static PixelDescription Half_HDR_RGB = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rgb16f,
        Format = PixelFormat.Rgb,
        Type = PixelType.Float
    };

    public readonly static PixelDescription Half_HDR_RG = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.Rg16f,
        Format = PixelFormat.Rg,
        Type = PixelType.Float
    };

    public readonly static PixelDescription Half_HDR_R = new PixelDescription
    {
        InternalFormat = SizedInternalFormat.R16f,
        Format = PixelFormat.Red,
        Type = PixelType.Float
    };
    #endregion
}