using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities.Textures;

public struct TextureFiltering
{
    public required TextureMinFilter MinFilter;
    public required TextureMagFilter MagFilter;
    public required bool GenerateMimap;

    public static readonly TextureFiltering Linear = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.Linear,
        MagFilter = TextureMagFilter.Linear,
        GenerateMimap = false 
    };
    public static readonly TextureFiltering Nearest = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.Nearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMimap = false 
    };
    public static readonly TextureFiltering LinearMipMapLinear = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.LinearMipmapLinear,
        MagFilter = TextureMagFilter.Linear,
        GenerateMimap = true 
    };
    public static readonly TextureFiltering LinearMipmapNearest = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.LinearMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMimap = true 
    };
    public static readonly TextureFiltering NearestMipmapLinear = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.NearestMipmapLinear,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMimap = true 
    };
    public static readonly TextureFiltering NearestMipmapNearest = new TextureFiltering
    { 
        MinFilter = TextureMinFilter.NearestMipmapNearest,
        MagFilter = TextureMagFilter.Nearest,
        GenerateMimap = true 
    };
}
