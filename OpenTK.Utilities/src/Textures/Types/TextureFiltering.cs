using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct TextureFiltering
{
    public static readonly TextureFiltering Linear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Linear,
        MagFilter = TextureMagFilter.Linear,
        GenerateMipmap = false,
    };

    public static readonly TextureFiltering Nearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Nearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMipmap = false,
    };

    public static readonly TextureFiltering LinearNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Linear,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMipmap = false,
    };

    public static readonly TextureFiltering NearestLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Nearest,
        MagFilter = TextureMagFilter.Linear,
        GenerateMipmap = false,
    };

    public static readonly TextureFiltering LinearMipMapLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.LinearMipmapLinear,
        MagFilter = TextureMagFilter.Linear,
        GenerateMipmap = true,
    };

    public static readonly TextureFiltering LinearMipmapNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.LinearMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMipmap = true,
    };

    public static readonly TextureFiltering NearestMipmapLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.NearestMipmapLinear,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMipmap = true,
    };

    public static readonly TextureFiltering NearestMipmapNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.NearestMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMipmap = true,
    };

    public TextureFiltering(bool gerateMipmap, TextureMinFilter TextureMinFilter, TextureMagFilter TextureMagFilter)
    {
        this.GenerateMipmap = gerateMipmap;
        this.MinFilter = TextureMinFilter;
        this.MagFilter = TextureMagFilter;
    }

    public bool GenerateMipmap { get; set; }

    public TextureMinFilter MinFilter { get; set; }

    public TextureMagFilter MagFilter { get; set; }
}
