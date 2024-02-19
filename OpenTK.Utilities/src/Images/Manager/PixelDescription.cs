using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Images;

#pragma warning disable SA1310 // Field names should not contain underscore
public struct PixelDescription
{
    #region Template Sample
    public static readonly PixelDescription RGBA = new PixelDescription
    {
        TexFormat = TextureFormat.Rgba8,
        PxFormat = PixelFormat.Rgba,
        PxType = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription RGB = new PixelDescription
    {
        TexFormat = TextureFormat.Rgb8,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription RG = new PixelDescription
    {
        TexFormat = TextureFormat.Rg8,
        PxFormat = PixelFormat.Rg,
        PxType = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription R = new PixelDescription
    {
        TexFormat = TextureFormat.R8,
        PxFormat = PixelFormat.Red,
        PxType = PixelType.UnsignedByte,
    };
    #endregion

    #region Template Sample Gamma
    public static readonly PixelDescription SRGBA = new PixelDescription
    {
        TexFormat = TextureFormat.Srgb8Alpha8,
        PxFormat = PixelFormat.Rgba,
        PxType = PixelType.UnsignedByte,
    };

    public static readonly PixelDescription SRGB = new PixelDescription
    {
        TexFormat = TextureFormat.Srgb8,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.UnsignedByte,
    };
    #endregion

    #region Template HDR
    public static readonly PixelDescription HDR_RGBA = new PixelDescription
    {
        TexFormat = TextureFormat.Rgba32f,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HDR_RGB = new PixelDescription
    {
        TexFormat = TextureFormat.Rgb32f,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HDR_RG = new PixelDescription
    {
        TexFormat = TextureFormat.Rg32f,
        PxFormat = PixelFormat.Rg,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HDR_R = new PixelDescription
    {
        TexFormat = TextureFormat.R32f,
        PxFormat = PixelFormat.Red,
        PxType = PixelType.Float,
    };
    #endregion

    #region Template HALF HDR
    public static readonly PixelDescription HALF_HDR_RGBA = new PixelDescription
    {
        TexFormat = TextureFormat.Rgba16f,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HALF_HDR_RGB = new PixelDescription
    {
        TexFormat = TextureFormat.Rgb16f,
        PxFormat = PixelFormat.Rgb,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HALF_HDR_RG = new PixelDescription
    {
        TexFormat = TextureFormat.Rg16f,
        PxFormat = PixelFormat.Rg,
        PxType = PixelType.Float,
    };

    public static readonly PixelDescription HALF_HDR_R = new PixelDescription
    {
        TexFormat = TextureFormat.R16f,
        PxFormat = PixelFormat.Red,
        PxType = PixelType.Float,
    };
    #endregion

    public TextureFormat TexFormat { get; set; }

    public PixelFormat PxFormat { get; set; }

    public PixelType PxType { get; set; }

    #region ProcessFormats
    public static int CountBytesPerPxType(PixelType PixelType)
    {
        return PixelType switch
        {
            PixelType.Byte => 1,
            PixelType.UnsignedByte => 1,
            PixelType.Short => 2,
            PixelType.UnsignedShort => 2,
            PixelType.Int => 4,
            PixelType.UnsignedInt => 4,
            PixelType.Float => 4,
            PixelType.HalfFloat => 2,
            PixelType.UnsignedByte332 => 1,
            PixelType.UnsignedShort4444 => 2,
            PixelType.UnsignedShort5551 => 2,
            PixelType.UnsignedInt8888 => 4,
            PixelType.UnsignedInt1010102 => 4,
            PixelType.UnsignedByte233Reversed => 1,
            PixelType.UnsignedShort565 => 2,
            PixelType.UnsignedShort4444Reversed => 2,
            PixelType.UnsignedShort1555Reversed => 2,
            PixelType.UnsignedInt8888Reversed => 4,
            PixelType.UnsignedInt2101010Reversed => 4,
            PixelType.UnsignedInt248 => 4,
            PixelType.UnsignedInt10F11F11FRev => 4,
            PixelType.UnsignedInt5999Rev => 4,
            PixelType.Float32UnsignedInt248Rev => 8,
            _ => 0
        };
    }

    public static int CountChannelsPerPxFormat(PixelFormat PixelFormat)
    {
        return PixelFormat switch
        {
            PixelFormat.UnsignedShort => 1,
            PixelFormat.UnsignedInt => 1,
            PixelFormat.ColorIndex => 1,
            PixelFormat.StencilIndex => 1,
            PixelFormat.DepthComponent => 1,
            PixelFormat.Red => 1,
            PixelFormat.Green => 1,
            PixelFormat.Blue => 1,
            PixelFormat.Alpha => 1,
            PixelFormat.Rgb => 3,
            PixelFormat.Rgba => 4,
            PixelFormat.Luminance => 1,
            PixelFormat.LuminanceAlpha => 2,
            PixelFormat.AbgrExt => 4,
            PixelFormat.CmykExt => 4,
            PixelFormat.CmykaExt => 5,
            PixelFormat.Bgr => 3,
            PixelFormat.Bgra => 4,
            PixelFormat.Ycrcb422Sgix => 1,
            PixelFormat.Ycrcb444Sgix => 1,
            PixelFormat.Rg => 2,
            PixelFormat.RgInteger => 2,
            PixelFormat.R5G6B5IccSgix => 3,
            PixelFormat.R5G6B5A8IccSgix => 4,
            PixelFormat.Alpha16IccSgix => 1,
            PixelFormat.Luminance16IccSgix => 1,
            PixelFormat.Luminance16Alpha8IccSgix => 2,
            PixelFormat.DepthStencil => 1,
            PixelFormat.RedInteger => 1,
            PixelFormat.GreenInteger => 1,
            PixelFormat.BlueInteger => 1,
            PixelFormat.AlphaInteger => 1,
            PixelFormat.RgbInteger => 3,
            PixelFormat.RgbaInteger => 4,
            PixelFormat.BgrInteger => 3,
            PixelFormat.BgraInteger => 4,
            _ => 0,
        };
    }
    #endregion

    public static PixelDescription GetPixelDesc(DescType DescType, Channels Channels)
    {
        return DescType switch
        {
            DescType.Default => Channels switch
            {
                Channels.RedGreenBlueAlpha => RGBA,
                Channels.RedGreenBlue => RGB,
                Channels.GreyAlpha => RG,
                Channels.Grey => R,
                _ => RGB,
            },

            DescType.Gamma => Channels switch
            {
                Channels.RedGreenBlueAlpha => SRGBA,
                Channels.RedGreenBlue => SRGB,
                Channels.GreyAlpha => RG,
                Channels.Grey => R,
                _ => SRGB,
            },

            DescType.HalfHdr => Channels switch
            {
                Channels.RedGreenBlueAlpha => HDR_RGBA,
                Channels.RedGreenBlue => HDR_RGB,
                Channels.GreyAlpha => HDR_RG,
                Channels.Grey => HDR_R,
                _ => HDR_RGB,
            },

            DescType.Hdr => Channels switch
            {
                Channels.RedGreenBlueAlpha => HALF_HDR_RGBA,
                Channels.RedGreenBlue => HALF_HDR_RGB,
                Channels.GreyAlpha => HALF_HDR_RG,
                Channels.Grey => HALF_HDR_R,
                _ => HALF_HDR_RGB,
            },

            _ => default
        };
    }
}
#pragma warning restore SA1310 // Field names should not contain underscore
