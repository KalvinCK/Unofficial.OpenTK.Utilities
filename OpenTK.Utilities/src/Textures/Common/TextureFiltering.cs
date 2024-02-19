using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct TextureFiltering
{
    public static readonly TextureFiltering Linear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Linear,
        MagFilter = TextureMagFilter.Linear,
    };

    public static readonly TextureFiltering Nearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Nearest,
        MagFilter = TextureMagFilter.Nearest,
    };

    public static readonly TextureFiltering LinearNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Linear,
        MagFilter = TextureMagFilter.Nearest,
    };

    public static readonly TextureFiltering NearestLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.Nearest,
        MagFilter = TextureMagFilter.Linear,
    };

    public static readonly TextureFiltering LinearMipMapLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.LinearMipmapLinear,
        MagFilter = TextureMagFilter.Linear,
    };

    public static readonly TextureFiltering LinearMipmapNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.LinearMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
    };

    public static readonly TextureFiltering NearestMipmapLinear = new TextureFiltering
    {
        MinFilter = TextureMinFilter.NearestMipmapLinear,
        MagFilter = TextureMagFilter.Nearest,
    };

    public static readonly TextureFiltering NearestMipmapNearest = new TextureFiltering
    {
        MinFilter = TextureMinFilter.NearestMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
    };

    public TextureFiltering(TextureMinFilter TextureMinFilter, TextureMagFilter TextureMagFilter)
    {
        this.MinFilter = TextureMinFilter;
        this.MagFilter = TextureMagFilter;
    }

    public TextureMinFilter MinFilter { get; set; }

    public TextureMagFilter MagFilter { get; set; }

    public override readonly string ToString()
    {
        return $"MinFilter: {this.MinFilter}, MagFilter: {this.MagFilter}";
    }
}
