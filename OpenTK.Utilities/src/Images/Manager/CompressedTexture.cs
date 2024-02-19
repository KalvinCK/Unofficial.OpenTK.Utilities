using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Images;

public static class CompressedTexture
{
    public static Texture2D LoadTexture(string imagepath)
    {
        const uint FourccDXT1 = 0x31545844; // Equivalent to "DXT1" in ASCII
        const uint FourccDXT3 = 0x33545844; // Equivalent to "DXT3" in ASCII
        const uint FourccDXT5 = 0x35545844; // Equivalent to "DXT5" in ASCII

        byte[] header = new byte[124];

        using FileStream fp = File.OpenRead(imagepath);

        byte[] filecode = new byte[4];
        fp.Read(filecode, 0, 4);

        if (BitConverter.ToUInt32(filecode, 0) != 0x20534444) // Equivalent to "DDS " in ASCII
        {
            fp.Close();
            throw new FormatException("Format file not '.dds'");
        }

        fp.Read(header, 0, 124);

        uint height = BitConverter.ToUInt32(header, 8);
        uint width = BitConverter.ToUInt32(header, 12);
        uint linearSize = BitConverter.ToUInt32(header, 16);
        uint mipMapCount = BitConverter.ToUInt32(header, 24);
        uint fourCC = BitConverter.ToUInt32(header, 80);

        byte[] buffer;
        uint bufsize;

        bufsize = mipMapCount > 1 ? linearSize * 2 : linearSize;
        buffer = new byte[bufsize];
        fp.Read(buffer, 0, (int)bufsize);

        fp.Close();

        uint components = fourCC == FourccDXT1 ? 3u : 4u;
        var format = fourCC switch
        {
            FourccDXT1 => TextureFormat.CompressedRgbS3tcDxt1,
            FourccDXT3 => TextureFormat.CompressedRgbaS3tcDxt3,
            FourccDXT5 => TextureFormat.CompressedRgbaS3tcDxt5,
            _ => throw new FormatException("Format file not suported"),
        };
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

        uint blockSize = format == TextureFormat.CompressedRgbaS3tcDxt1 ? 8u : 16u;
        uint offset = 0;

        Texture2D texture = new Texture2D();
        var span = new Span<byte>(buffer);

        /* load the mipmaps */
        for (uint level = 0; level < mipMapCount && (width > 0 || height > 0); ++level)
        {
            uint size = (width + 3) / 4 * ((height + 3) / 4) * blockSize;

            if (level is 0)
            {
                texture.AllocateStorage(format, (int)width, (int)height, (int)mipMapCount);
            }

            texture.UpdateCompress((int)width, (int)height,
                format == TextureFormat.CompressedRgbaS3tcDxt5 ? PixelFormat.Rgba : PixelFormat.Rgb,
                (int)size, span[(int)offset], (int)level);

            offset += size;
            width /= 2;
            height /= 2;

            // Deal with Non-Power-Of-Two textures. This code is not included here.
            width = Math.Max(width, 1);
            height = Math.Max(height, 1);
        }

        // You would need to replace this with the appropriate memory management in your C# environment
        // For demonstration purposes, I'm just leaving it here
        buffer = [];
        Array.Clear(buffer);

        return texture;
    }
}
