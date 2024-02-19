using System.Runtime.InteropServices;

namespace OpenTK.Utilities.Images;

public sealed unsafe class ImageWrite : IDisposable
{
    private Stream stream = Stream.Null;
    private byte[] buffer = new byte[1024];

    public void WriteBmp(void* data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        try
        {
            this.stream = dest;
            ImageWriteCore.WriteBmpToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, data);
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WriteBmp(byte[] data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        CheckParams(data, width, height, channelsWriter);

        fixed (byte* b = &data[0])
        {
            this.WriteBmp(b, width, height, channelsWriter, dest);
        }
    }

    public void WriteTga(void* data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        try
        {
            this.stream = dest;
            ImageWriteCore.WriteTgaToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, data);
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WriteTga(byte[] data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        CheckParams(data, width, height, channelsWriter);

        fixed (byte* b = &data[0])
        {
            this.WriteTga(b, width, height, channelsWriter, dest);
        }
    }

    public void WriteHdr(void* data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        try
        {
            this.stream = dest;
            ImageWriteCore.WriteHdrToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, (float*)data);
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WriteHdr(byte[] data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        CheckParams(data, width, height, channelsWriter);

        try
        {
            this.stream = dest;
            var f = new float[data.Length];
            for (var i = 0; i < data.Length; ++i)
            {
                f[i] = data[i] / 255.0f;
            }

            fixed (float* fptr = f)
            {
                ImageWriteCore.WriteHdrToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, fptr);
            }
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WritePng(void* data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        try
        {
            this.stream = dest;

            ImageWriteCore.WritePngToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, data,
               width * (int)channelsWriter);
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WritePng(byte[] data, int width, int height, ChannelsWrite channelsWriter, Stream dest)
    {
        CheckParams(data, width, height, channelsWriter);

        fixed (byte* b = &data[0])
        {
            this.WritePng(b, width, height, channelsWriter, dest);
        }
    }

    public void WriteJpg(void* data, int width, int height, ChannelsWrite channelsWriter, Stream dest, int quality)
    {
        try
        {
            this.stream = dest;

            ImageWriteCore.WriteJpgToFunc(this.WriteCallback, null, width, height, (int)channelsWriter, data, quality);
        }
        finally
        {
            this.stream = Stream.Null;
        }
    }

    public void WriteJpg(byte[] data, int width, int height, ChannelsWrite channelsWriter, Stream dest, int quality)
    {
        CheckParams(data, width, height, channelsWriter);

        fixed (byte* b = &data[0])
        {
            this.WriteJpg(b, width, height, channelsWriter, dest, quality);
        }
    }

    public void Dispose()
    {
        this.stream = Stream.Null;
        this.buffer = [];
    }

    private static void CheckParams(byte[] data, int width, int height, ChannelsWrite channelsWriter)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(width);
        ArgumentNullException.ThrowIfNull(height);

        int requiredDataSize = width * height * (int)channelsWriter;
        if (data.Length < requiredDataSize)
        {
            throw new ArgumentException(
                string.Format("Not enough data. 'data' variable should contain at least {0} bytes.", requiredDataSize));
        }
    }

    private void WriteCallback(void* context, void* data, int size)
    {
        if (data == null || size <= 0)
        {
            return;
        }

        if (this.buffer.Length < size)
        {
            this.buffer = new byte[size * 2];
        }

        var bptr = (byte*)data;

        Marshal.Copy(new IntPtr(bptr), this.buffer, 0, size);

        this.stream.Write(this.buffer, 0, size);
    }
}
