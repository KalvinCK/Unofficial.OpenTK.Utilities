using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using StbImageWriteSharp;
using ColorComponents = StbImageSharp.ColorComponents;

namespace OpenTK.Utilities.Textures;

public static class TextureManager
{
    #region Save
    public static unsafe void SaveJpg(Texture2D texture, string filePath, string fileName, int quality = 100, bool flipVertically = true)
    {
        StbImageWrite.stbi_flip_vertically_on_write(flipVertically ? 1 : 0);
        using FileStream fileStream = File.OpenWrite($"{Path.Combine(filePath, fileName)}.jpg");

        int bufSize = texture.Width * texture.Height * 3;
        byte[] pixels = new byte[bufSize];
        fixed (void* ptr = pixels)
        {
            texture.GetImageData(
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                (IntPtr)ptr,
                bufSize * sizeof(byte));
        }

        var imageWriter = new ImageWriter();
        imageWriter.WriteJpg(pixels, texture.Width, texture.Height, StbImageWriteSharp.ColorComponents.RedGreenBlue, fileStream, quality);

        Array.Clear(pixels);
        StbImageWrite.stbi_flip_vertically_on_write(0);
    }

    public static unsafe void SavePNG(Texture2D texture, string filePath, string fileName, bool flipVertically = true)
    {
        StbImageWrite.stbi_flip_vertically_on_write(flipVertically ? 1 : 0);
        using FileStream fileStream = File.OpenWrite($"{Path.Combine(filePath, fileName)}.png");

        int bufSize = texture.Width * texture.Height * 4;
        byte[] pixels = new byte[bufSize];
        fixed (void* ptr = pixels)
        {
            texture.GetImageData(
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                (IntPtr)ptr,
                bufSize);
        }

        var imageWriter = new ImageWriter();
        imageWriter.WritePng(pixels, texture.Width, texture.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, fileStream);

        Array.Clear(pixels);
        StbImageWrite.stbi_flip_vertically_on_write(0);
    }
    #endregion

    #region Loads
    public static PixelData<byte> Load(string imagePath, Channels channels = Channels.Default, bool flipVertically = true)
    {
        using var stream = File.OpenRead(imagePath);
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        ImageResult imageResult = ImageResult.FromStream(stream, (ColorComponents)channels);
        StbImage.stbi_set_flip_vertically_on_load(0);

        return new PixelData<byte>(imageResult.Width, imageResult.Height, imageResult.Data, (Channels)imageResult.SourceComp);
    }

    public static PixelData<float> LoadHdr(string imagePath, Channels channels = Channels.Default, bool flipVertically = true)
    {
        using var stream = File.OpenRead(imagePath);
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        ImageResultFloat imageResult = ImageResultFloat.FromStream(stream, (ColorComponents)channels);
        StbImage.stbi_set_flip_vertically_on_load(0);

        return new PixelData<float>(imageResult.Width, imageResult.Height, imageResult.Data, (Channels)imageResult.SourceComp);
    }

    public static void Load(string imagePath, out PixelData<byte> pixel, Channels channels = Channels.Default, bool flipVertically = true)
    {
        using var stream = File.OpenRead(imagePath);
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        ImageResult imageResult = ImageResult.FromStream(stream, (ColorComponents)channels);
        StbImage.stbi_set_flip_vertically_on_load(0);

        pixel = new PixelData<byte>(imageResult.Width, imageResult.Height, imageResult.Data, (Channels)imageResult.SourceComp);
    }

    public static void LoadHdr(string imagePath, out PixelData<float> pixel, Channels channels = Channels.Default, bool flipVertically = true)
    {
        using var stream = File.OpenRead(imagePath);
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
        ImageResultFloat imageResult = ImageResultFloat.FromStream(stream, (ColorComponents)channels);
        StbImage.stbi_set_flip_vertically_on_load(0);

        pixel = new PixelData<float>(imageResult.Width, imageResult.Height, imageResult.Data, (Channels)imageResult.SourceComp);
    }
    #endregion

    public static Texture2D CreateTexFromFile(
        string imgFilePath,
        TextureFiltering filtering,
        Texture2DWrapping wraping,
        int numLevels,
        bool srgbSpace = true,
        bool flipVertically = true,
        Channels channels = Channels.Default)
    {
        Load(imgFilePath, out var img, channels, flipVertically);
        var description = img.GetPixelDesc(srgbSpace ? DescType.SrgbSpace : DescType.Default);

        var texture = new Texture2D(description.InternalFormat, img.Width, img.Height, numLevels);
        texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);

        texture.Wrapping = wraping;
        texture.Filtering = filtering;

        return texture;
    }

    public static Texture2D CreateTexHDRFromFile(
        string imgFilePath,
        TextureFiltering filtering,
        Texture2DWrapping wraping,
        int numLevels,
        bool useHalf,
        bool flipVertically = true,
        Channels channels = Channels.Default)
    {
        LoadHdr(imgFilePath, out var img, channels, flipVertically);
        var description = img.GetPixelDesc(useHalf ? DescType.HDR16 : DescType.HDR32);

        var texture = new Texture2D(description.InternalFormat, img.Width, img.Height, numLevels);
        texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);

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
            LoadHdr(imgFilePath, out var img, Channels.Default, flipVertically);
            var description = img.GetPixelDesc(DescType.HDR16);

            texture = new Texture2D(description.InternalFormat, img.Width, img.Height, numLevels);
            texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);
        }
        else
        {
            Load(imgFilePath, out var img, Channels.Default, flipVertically);
            var description = img.GetPixelDesc(srgbSpace ? DescType.SrgbSpace : DescType.Default);

            texture = new Texture2D(description.InternalFormat, img.Width, img.Height, numLevels);
            texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);
        }

        texture.Wrapping = Texture2DWrapping.ClampToEdge;

        if (numLevels > 1)
        {
            texture.Filtering = new TextureFiltering(true, TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
        }
        else
        {
            texture.Filtering = new (false, TextureMinFilter.Linear, TextureMagFilter.Linear);
        }

        return texture;
    }

    public static TextureRectangle CreateDefaulRectangleFromFile(string imgFilePath, bool srgbSpace = true, bool flipVertically = true)
    {
        TextureRectangle texture;

        string ext = Path.GetExtension(imgFilePath).ToUpper();
        if (ext is ".HDR")
        {
            LoadHdr(imgFilePath, out var img, Channels.Default, flipVertically);
            var description = img.GetPixelDesc(DescType.HDR32);

            texture = new TextureRectangle(description.InternalFormat, img.Width, img.Height);
            texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);
        }
        else
        {
            Load(imgFilePath, out var img, Channels.Default, flipVertically);
            var description = img.GetPixelDesc(srgbSpace ? DescType.SrgbSpace : DescType.Default);

            texture = new TextureRectangle(description.InternalFormat, img.Width, img.Height);
            texture.Update(img.Width, img.Height, description.Format, description.Type, img.Data);
        }

        return texture;
    }

    public static TextureCubeMap CreateDefaulCubeMapFromFile(CubemapTexturesPath texturesPath, bool srgbSpace = true, int numLevels = 1, bool flipVertically = true)
    {
        Debug.Assert(texturesPath.Files.Length == 6, "Must be 6 images.");

        TextureCubeMap texture = new TextureCubeMap();
        for (int i = 0; i < 6; i++)
        {
            Load(Path.Combine(texturesPath.RootPathTextures, texturesPath.Files[i]), out var img, Channels.Default, flipVertically);
            var description = img.GetPixelDesc(srgbSpace ? DescType.SrgbSpace : DescType.Default);

            if (i == 0)
            {
                texture.ToAllocate(description.InternalFormat, img.Width, img.Height, numLevels);
            }

            texture.Update(img.Width, img.Height, CubeMapLayer.PositiveX + i, description.Format, PixelType.UnsignedByte, img.Data);
        }

        texture.EnableSeamlessCubemapARB_AMD(true);
        texture.Wrapping = new Texture3DWrapping(TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge);

        if (numLevels > 1)
        {
            texture.Filtering = new TextureFiltering(true, TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
        }
        else
        {
            texture.Filtering = new TextureFiltering(false, TextureMinFilter.Linear, TextureMagFilter.Linear);
        }

        return texture;
    }
}
