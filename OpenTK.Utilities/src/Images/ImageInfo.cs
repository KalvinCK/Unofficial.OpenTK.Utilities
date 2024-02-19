namespace OpenTK.Utilities.Images;

public readonly struct ImageInfo(int width, int height, Channels channels, int bitsPC)
{
    public int Width { get; } = width;

    public int Height { get; } = height;

    public Channels Channels { get; } = channels;

    public int BitsPerChannel { get; } = bitsPC;

    public static unsafe ImageInfo? FromStream(Stream stream)
    {
        int width, height, comp;
        var context = new ImageCore.Context(stream);

        var is16Bit = ImageCore.Is16Main(context) == 1;
        ImageCore.Rewind(context);

        var infoResult = ImageCore.InfoMain(context, &width, &height, &comp);
        ImageCore.Rewind(context);

        if (infoResult == 0)
        {
            return null;
        }

        return new ImageInfo(width, height, (Channels)comp, is16Bit ? 16 : 8);
    }
}
