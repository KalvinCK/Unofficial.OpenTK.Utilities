using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images;

public class ImageFloat : IDisposable
{
    public ImageFloat() { }

    public ImageFloat(int width, int height, float[] data, Channels channels, Channels sourceChannels)
    {
        this.Width = width;
        this.Height = height;
        this.Data = data;
        this.Channels = channels;
        this.SourceChannels = sourceChannels;
    }

    public int Width { get; set; }

    public int Height { get; set; }

    public Channels SourceChannels { get; set; }

    public Channels Channels { get; set; }

    public float[] Data { get; set; } = [];

    public static unsafe ImageFloat FromStream(Stream stream, Channels requiredChannels = Channels.Default)
    {
        float* result = null;

        try
        {
            int x, y, channels;

            var context = new ImageCore.Context(stream);

            result = ImageCore.LoadfMain(context, &x, &y, &channels, (int)requiredChannels);

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

    public static ImageFloat FromFile(string filePath, bool flipVertically = true, Channels requiredChannels = Channels.Default)
    {
        ImageCore.SetFlipVerticallyOnLoad(flipVertically ? 1 : 0);
        using var stream = File.OpenRead(filePath);
        ImageFloat result = FromStream(stream, requiredChannels);
        ImageCore.SetFlipVerticallyOnLoad(0);
        return result;
    }

    public static ImageFloat FromMemory(byte[] data, Channels requiredChannels = Channels.Default)
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
            this.Width = 0;
            this.Height = 0;
            this.SourceChannels = Channels.Default;
            this.Channels = Channels.Default;
            this.Data = [];
        }
    }

    internal static unsafe ImageFloat FromResult(float* result, int width, int height, Channels channels, Channels reqChannels)
    {
        ArgumentNullException.ThrowIfNull(result, ImageCore.GFailureReason);

        var image = new ImageFloat
        {
            Width = width,
            Height = height,
            SourceChannels = channels,
            Channels = reqChannels == Channels.Default ? channels : reqChannels,
        };

        // Convert to array
        image.Data = new float[width * height * (int)image.Channels];
        Marshal.Copy(new IntPtr(result), image.Data, 0, image.Data.Length);

        return image;
    }
}
