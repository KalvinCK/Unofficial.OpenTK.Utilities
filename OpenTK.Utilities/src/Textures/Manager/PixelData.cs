namespace OpenTK.Utilities.Textures;

public ref struct PixelData<TPixels>(int width, int height, Span<TPixels> pixels, Channels channels)
    where TPixels : unmanaged
{
    public Span<TPixels> Data = pixels;
    public int Width = width;
    public int Height = height;
    public Channels Channels = channels;

    public readonly PixelDescription GetPixelDesc(DescType DescType)
    {
        return DescType switch
        {
            DescType.Default => this.Channels switch
            {
                Channels.RedGreenBlueAlpha => PixelDescription.RGBA,
                Channels.RedGreenBlue => PixelDescription.RGB,
                Channels.GreyAlpha => PixelDescription.RG,
                Channels.Grey => PixelDescription.R,
                _ => PixelDescription.RGB,
            },

            DescType.SrgbSpace => this.Channels switch
            {
                Channels.RedGreenBlueAlpha => PixelDescription.SRGBA,
                Channels.RedGreenBlue => PixelDescription.SRGB,
                Channels.GreyAlpha => PixelDescription.RG,
                Channels.Grey => PixelDescription.R,
                _ => PixelDescription.SRGB,
            },

            DescType.HDR16 => this.Channels switch
            {
                Channels.RedGreenBlueAlpha => PixelDescription.HDR_RGBA,
                Channels.RedGreenBlue => PixelDescription.HDR_RGB,
                Channels.GreyAlpha => PixelDescription.HDR_RG,
                Channels.Grey => PixelDescription.HDR_R,
                _ => PixelDescription.HDR_RGB,
            },

            DescType.HDR32 => this.Channels switch
            {
                Channels.RedGreenBlueAlpha => PixelDescription.Half_HDR_RGBA,
                Channels.RedGreenBlue => PixelDescription.Half_HDR_RGB,
                Channels.GreyAlpha => PixelDescription.Half_HDR_RG,
                Channels.Grey => PixelDescription.Half_HDR_R,
                _ => PixelDescription.Half_HDR_RGB,
            },

            _ => this.GetPixelDesc(DescType.Default)
        };
    }
}
