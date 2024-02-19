using System.Diagnostics;
using System.Net.NetworkInformation;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Utilities.Textures;

namespace OpenTK.Utilities.Images;

public static class TextureManager
{
    public static unsafe void SaveJpg<Tx2D>(Tx2D texture, string filePath, string fileName, int quality = 100, bool flipVertically = true)
        where Tx2D : ITexture2D
    {
        ImageWriteCore.FlipVerticallyOnWrite(flipVertically ? 1 : 0);

        using FileStream fileStream = File.OpenWrite($"{Path.Combine(filePath, fileName)}.jpg");

        int bufSize = texture.Width * texture.Height * 3;
        byte[] pixels = new byte[bufSize];
        fixed (void* ptr = pixels)
        {
            texture.GetImageData(
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                (nint)ptr,
                bufSize * sizeof(byte));
        }

        var imageWriter = new ImageWrite();
        imageWriter.WriteJpg(pixels, texture.Width, texture.Height, ChannelsWrite.RedGreenBlue, fileStream, quality);

        pixels = [];
        ImageWriteCore.FlipVerticallyOnWrite(0);
    }

    public static unsafe void SavePNG<Tx2D>(Tx2D texture, string filePath, string fileName, bool flipVertically = true)
        where Tx2D : ITexture2D
    {
        ImageWriteCore.FlipVerticallyOnWrite(flipVertically ? 1 : 0);
        using FileStream fileStream = File.OpenWrite($"{Path.Combine(filePath, fileName)}.png");

        int bufSize = texture.Width * texture.Height * 4;
        byte[] pixels = new byte[bufSize];
        fixed (void* ptr = pixels)
        {
            texture.GetImageData(
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                (nint)ptr,
                bufSize);
        }

        var imageWriter = new ImageWrite();
        imageWriter.WritePng(pixels, texture.Width, texture.Height, ChannelsWrite.RedGreenBlueAlpha, fileStream);

        pixels = [];
        ImageWriteCore.FlipVerticallyOnWrite(0);
    }

    public static Texture2D CreateTextureFromFile(
        string imgFilePath,
        TextureFiltering filtering,
        Texture2DWrapping wraping,
        int numLevels,
        bool srgbSpace = true,
        bool flipVertically = true,
        Channels requiredChannels = Channels.Default)
    {
        using Image img = Image.FromFile(imgFilePath, flipVertically, requiredChannels);
        PixelDescription description = PixelDescription.GetPixelDesc(srgbSpace ? DescType.Gamma : DescType.Default, img.SourceChannels);

        Texture2D texture = new Texture2D(description.TexFormat, img.Width, img.Height, numLevels);
        texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);

        texture.Wrapping = wraping;
        texture.Filtering = filtering;

        return texture;
    }

    public static Texture2D CreateTexHDRFromFile(
        string imgFilePath,
        TextureFiltering filtering,
        Texture2DWrapping wraping,
        int numLevels,
        bool useHalfHdr,
        bool flipVertically = true,
        Channels requiredChannels = Channels.Default)
    {
        ImageFloat img = ImageFloat.FromFile(imgFilePath, flipVertically, requiredChannels);
        PixelDescription description = PixelDescription.GetPixelDesc(useHalfHdr ? DescType.HalfHdr : DescType.Hdr, img.SourceChannels);

        Texture2D texture = new Texture2D(description.TexFormat, img.Width, img.Height, numLevels);
        texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);

        texture.Wrapping = wraping;
        texture.Filtering = filtering;

        return texture;
    }

    public static Texture2D CreateDefaultTexture2DFromFile(string imgFilePath, bool srgbSpace = true, int numLevels = 1, bool flipVertically = true)
    {
        Texture2D texture;

        string ext = Path.GetExtension(imgFilePath).ToUpper();
        if (ext is ".HDR")
        {
            using ImageFloat img = ImageFloat.FromFile(imgFilePath, flipVertically);
            PixelDescription description = PixelDescription.GetPixelDesc(DescType.HalfHdr, img.SourceChannels);

            texture = new Texture2D(description.TexFormat, img.Width, img.Height, numLevels);
            texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);
        }
        else
        {
            using Image img = Image.FromFile(imgFilePath, flipVertically);
            PixelDescription description = PixelDescription.GetPixelDesc(srgbSpace ? DescType.Gamma : DescType.Default, img.SourceChannels);

            texture = new Texture2D(description.TexFormat, img.Width, img.Height, numLevels);
            texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);
        }

        texture.Wrapping = Texture2DWrapping.ClampToEdge;

        if (numLevels > 1)
        {
            texture.Filtering = TextureFiltering.LinearMipMapLinear;
            texture.GenerateMipmap();
        }
        else
        {
            texture.Filtering = TextureFiltering.Linear;
        }

        return texture;
    }

    public static TextureRectangle CreateDefaulRectangleFromFile(string imgFilePath, bool srgbSpace = true, bool flipVertically = true)
    {
        TextureRectangle texture;

        string ext = Path.GetExtension(imgFilePath).ToUpper();
        if (ext is ".HDR")
        {
            using ImageFloat img = ImageFloat.FromFile(imgFilePath, flipVertically);
            PixelDescription description = PixelDescription.GetPixelDesc(DescType.HalfHdr, img.SourceChannels);

            texture = new TextureRectangle(description.TexFormat, img.Width, img.Height);
            texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);
        }
        else
        {
            using Image img = Image.FromFile(imgFilePath, flipVertically);
            PixelDescription description = PixelDescription.GetPixelDesc(srgbSpace ? DescType.Gamma : DescType.Default, img.SourceChannels);

            texture = new TextureRectangle(description.TexFormat, img.Width, img.Height);
            texture.Update(img.Width, img.Height, description.PxFormat, description.PxType, img.Data);
        }

        return texture;
    }

    public static TextureCubeMap CreateDefaulCubeMapFromFile(CubeTexturesPack texturesPath, bool srgbSpace = true, int numLevels = 1, bool flipVertically = true)
    {
        Debug.Assert(texturesPath.Files.Length == 6, "Must be 6 images.");

        TextureCubeMap texture = new TextureCubeMap();
        for (int i = 0; i < 6; i++)
        {
            using Image img = Image.FromFile(Path.Combine(texturesPath.RootPathTextures, texturesPath.Files[i]));
            PixelDescription description = PixelDescription.GetPixelDesc(srgbSpace ? DescType.Gamma : DescType.Default, img.SourceChannels);

            if (i == 0)
            {
                texture.AllocateStorage(description.TexFormat, img.Width, img.Height, numLevels);
            }

            texture.Update(img.Width, img.Height, CubeMapLayer.PositiveX + i, description.PxFormat, PixelType.UnsignedByte, img.Data);
        }

        texture.EnableSeamlessCubemapARB_AMD(true);
        texture.Wrapping = new Texture3DWrapping(TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge);

        if (numLevels > 1)
        {
            texture.Filtering = TextureFiltering.LinearMipMapLinear;
            texture.GenerateMipmap();
        }
        else
        {
            texture.Filtering = TextureFiltering.Linear;
        }

        return texture;
    }
}
