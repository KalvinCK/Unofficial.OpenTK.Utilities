using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace OpenTK.Utilities.Images;

public class ImageAnimatedFrame : Image
{
    public int DelayInMs { get; set; }

    public static IEnumerable<ImageAnimatedFrame> AnimatedGifFramesFromStream(
        Stream stream,
        Channels requiredComponents = Channels.Default)
    {
        return new AnimatedGifEnumerable(stream, requiredComponents);
    }
}

internal class AnimatedGifEnumerator : IEnumerator<ImageAnimatedFrame>
{
    private readonly ImageCore.Context context;
    private ImageCore.Gif gif;

    public AnimatedGifEnumerator(Stream input, Channels colorComponents)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        this.context = new ImageCore.Context(input);

        if (ImageCore.GifTest(this.context) == 0)
        {
            throw new Exception("Input stream is not GIF file.");
        }

        this.gif = new ImageCore.Gif();
        this.Channels = colorComponents;
    }

    ~AnimatedGifEnumerator()
    {
        this.Dispose(false);
    }

    public Channels Channels { get; }

    public ImageAnimatedFrame Current { get; private set; }

    object IEnumerator.Current => this.Current;

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public unsafe bool MoveNext()
    {
        // Read next frame
        int ccomp;
        byte two_back;
        var result = ImageCore.GifLoadNext(this.context, this.gif, &ccomp, (int)this.Channels, &two_back);
        if (result == null)
        {
            return false;
        }

        if (this.Current == null)
        {
            this.Current = new ImageAnimatedFrame
            {
                Width = this.gif.w,
                Height = this.gif.h,
                SourceChannels = (Channels)ccomp,
                Channels = this.Channels == Channels.Default ? (Channels)ccomp : this.Channels,
            };

            this.Current.Data = new byte[this.Current.Width * this.Current.Height * (int)this.Current.Channels];
        }

        this.Current.DelayInMs = this.gif.delay;

        Marshal.Copy(new IntPtr(result), this.Current.Data, 0, this.Current.Data.Length);

        return true;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    protected unsafe virtual void Dispose(bool disposing)
    {
        if (this.gif != null)
        {
            if (this.gif._out_ != null)
            {
                CRuntime.free(this.gif._out_);
                this.gif._out_ = null;
            }

            if (this.gif.history != null)
            {
                CRuntime.free(this.gif.history);
                this.gif.history = null;
            }

            if (this.gif.background != null)
            {
                CRuntime.free(this.gif.background);
                this.gif.background = null;
            }

            this.gif = null;
        }
    }
}

internal class AnimatedGifEnumerable : IEnumerable<ImageAnimatedFrame>
{
    private readonly Stream input;

    public AnimatedGifEnumerable(Stream input, Channels colorComponents)
    {
        this.input = input;
        this.Channels = colorComponents;
    }

    public Channels Channels { get; }

    public IEnumerator<ImageAnimatedFrame> GetEnumerator()
    {
        return new AnimatedGifEnumerator(this.input, this.Channels);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
