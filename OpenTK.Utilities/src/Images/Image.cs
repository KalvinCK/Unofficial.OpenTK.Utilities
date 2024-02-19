using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images;

public class Image : IDisposable
{
    public Image() { }

    public Image(int width, int height, Channels channels, Channels sourceChannels, byte[] datas)
    {
        this.Width = width;
        this.Height = height;
        this.Data = datas;
        this.Channels = channels;
        this.SourceChannels = sourceChannels;
    }

    public int Width { get; set; }

    public int Height { get; set; }

    public Channels SourceChannels { get; set; }

    public Channels Channels { get; set; }

    public byte[] Data = [];

    public static unsafe Image FromStream(Stream stream, Channels requiredChannels = Channels.Default)
    {
        byte* result = null;

        try
        {
            int x, y, channels;

            var context = new ImageCore.Context(stream);

            result = ImageCore.LoadAndPostprocess8bit(context, &x, &y, &channels, (int)requiredChannels);

            return FromResult(result, x, y, (Channels)channels, requiredChannels);
        }
        finally
        {
            if (result != null)
            {
                CRuntime.free(result);
            }
        }
    }

    public static Image FromFile(string filePath, bool flipVertically = true, Channels requiredChannels = Channels.Default)
    {
        ImageCore.SetFlipVerticallyOnLoad(flipVertically ? 1 : 0);
        using var stream = File.OpenRead(filePath);
        Image result = FromStream(stream, requiredChannels);
        ImageCore.SetFlipVerticallyOnLoad(0);
        return result;
    }

    public static Image FromMemory(byte[] data, Channels requiredChannels = Channels.Default)
    {
        using var stream = new MemoryStream(data);
        return FromStream(stream, requiredChannels);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            disposing = true;
            this.Width = 0;
            this.Height = 0;
            this.SourceChannels = Channels.Default;
            this.Channels = Channels.Default;
            this.Data = [];
        }
    }

    internal static unsafe Image FromResult(byte* result, int width, int height, Channels channels, Channels reqChannels)
    {
        ArgumentNullException.ThrowIfNull(result, ImageCore.GFailureReason);

        var image = new Image
        {
            Width = width,
            Height = height,
            SourceChannels = channels,
            Channels = reqChannels == Channels.Default ? channels : reqChannels,
        };

        // Convert to array
        image.Data = new byte[width * height * (int)image.Channels];
        Marshal.Copy(new IntPtr(result), image.Data, 0, image.Data.Length);

        return image;
    }
}
